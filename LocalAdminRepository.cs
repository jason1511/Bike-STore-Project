using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Bike_STore_Project
{
    public sealed class LocalAdminRepository
    {
        public DataTable GetAvailableProducts()
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT p.id, p.brand, p.type, COALESCE(p.color,'') AS color,
       COALESCE(p.sell_price,0) AS sell_price, COALESCE(SUM(l.qty_remaining),0) AS available
FROM products p
LEFT JOIN stock_lots l ON l.product_id=p.id
WHERE COALESCE(p.is_active,1)=1
GROUP BY p.id, p.brand, p.type, p.color, p.sell_price
ORDER BY p.brand, p.type, p.color;";
            using var reader = cmd.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        public string CreateInvoice(
            string customerName,
            string? customerPhone,
            string? customerAddress,
            string paymentMethod,
            string? paymentBank,
            string? notes,
            IReadOnlyCollection<InvoiceDraftItem> items)
        {
            if (!AppSession.IsSignedIn) throw new InvalidOperationException("Sign in before creating an invoice.");
            if (string.IsNullOrWhiteSpace(customerName)) throw new ArgumentException("Customer name is required.");
            if (items.Count == 0) throw new ArgumentException("Add at least one invoice item.");
            if (items.Any(x => x.Quantity <= 0 || x.UnitPrice <= 0))
                throw new ArgumentException("Every item requires a positive quantity and unit price.");

            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();
            try
            {
                foreach (var grouped in items.GroupBy(x => x.ProductId))
                {
                    var requested = grouped.Sum(x => x.Quantity);
                    var available = GetAvailable(conn, tx, grouped.Key);
                    if (requested > available)
                        throw new InvalidOperationException($"Insufficient stock for {grouped.First().Bike}. Available: {available}.");
                }

                var now = DateTime.Now;
                var invoiceNumber = NextInvoiceNumber(conn, tx, now);
                long invoiceId;
                using (var header = conn.CreateCommand())
                {
                    header.Transaction = tx;
                    header.CommandText = @"
INSERT INTO invoices
(invoice_number, customer_name, customer_phone, customer_address, payment_method, payment_bank,
 notes, status, created_by_user_id, created_by_username, created_at)
VALUES ($number,$name,$phone,$address,$method,$bank,$notes,'ACTIVE',$uid,$user,$at);
SELECT last_insert_rowid();";
                    header.Parameters.AddWithValue("$number", invoiceNumber);
                    header.Parameters.AddWithValue("$name", customerName.Trim());
                    header.Parameters.AddWithValue("$phone", Db(customerPhone));
                    header.Parameters.AddWithValue("$address", Db(customerAddress));
                    header.Parameters.AddWithValue("$method", string.IsNullOrWhiteSpace(paymentMethod) ? "CASH" : paymentMethod.Trim().ToUpperInvariant());
                    header.Parameters.AddWithValue("$bank", Db(paymentBank));
                    header.Parameters.AddWithValue("$notes", Db(notes));
                    header.Parameters.AddWithValue("$uid", AppSession.UserId);
                    header.Parameters.AddWithValue("$user", AppSession.Username);
                    header.Parameters.AddWithValue("$at", now.ToString("yyyy-MM-dd HH:mm:ss"));
                    invoiceId = Convert.ToInt64(header.ExecuteScalar() ?? 0L);
                }

                foreach (var item in items)
                    InsertInvoiceItem(conn, tx, invoiceId, customerName.Trim(), now, item);

                WriteAudit(conn, tx, "CREATE_INVOICE", "invoices", invoiceId,
                    $"{invoiceNumber}; customer={customerName.Trim()}; items={items.Count}; total={items.Sum(x => x.LineTotal)}");
                tx.Commit();
                return invoiceNumber;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        public void VoidInvoice(int invoiceId, string reason)
        {
            if (!AppSession.IsAdmin) throw new InvalidOperationException("Admin access is required to void an invoice.");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("A void reason is required.");

            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();
            try
            {
                string number;
                string status;
                using (var getHeader = conn.CreateCommand())
                {
                    getHeader.Transaction = tx;
                    getHeader.CommandText = "SELECT invoice_number,status FROM invoices WHERE id=$id;";
                    getHeader.Parameters.AddWithValue("$id", invoiceId);
                    using var reader = getHeader.ExecuteReader();
                    if (!reader.Read()) throw new InvalidOperationException("Invoice not found.");
                    number = reader.GetString(0);
                    status = reader.GetString(1);
                }
                if (status.Equals("VOID", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Invoice is already void.");

                var rows = new List<(int SaleId, int ProductId, int Quantity)>();
                using (var items = conn.CreateCommand())
                {
                    items.Transaction = tx;
                    items.CommandText = "SELECT sale_id,product_id,quantity FROM invoice_items WHERE invoice_id=$id;";
                    items.Parameters.AddWithValue("$id", invoiceId);
                    using var reader = items.ExecuteReader();
                    while (reader.Read()) rows.Add((reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2)));
                }

                foreach (var row in rows)
                {
                    var before = GetAvailable(conn, tx, row.ProductId);
                    var consumed = new List<(int LotId, int Qty)>();
                    using (var lines = conn.CreateCommand())
                    {
                        lines.Transaction = tx;
                        lines.CommandText = "SELECT stock_lot_id,qty_sold FROM sale_lines WHERE sale_id=$sale;";
                        lines.Parameters.AddWithValue("$sale", row.SaleId);
                        using var reader = lines.ExecuteReader();
                        while (reader.Read()) consumed.Add((reader.GetInt32(0), reader.GetInt32(1)));
                    }
                    foreach (var line in consumed)
                    {
                        using var restore = conn.CreateCommand();
                        restore.Transaction = tx;
                        restore.CommandText = "UPDATE stock_lots SET qty_remaining=qty_remaining+$qty WHERE id=$lot;";
                        restore.Parameters.AddWithValue("$qty", line.Qty);
                        restore.Parameters.AddWithValue("$lot", line.LotId);
                        restore.ExecuteNonQuery();
                    }
                    using (var sale = conn.CreateCommand())
                    {
                        sale.Transaction = tx;
                        sale.CommandText = "UPDATE sales SET voided=1 WHERE id=$id;";
                        sale.Parameters.AddWithValue("$id", row.SaleId);
                        sale.ExecuteNonQuery();
                    }
                    InsertMovement(conn, tx, row.ProductId, null, invoiceId, "VOID_RESTORE", row.Quantity,
                        before, before + row.Quantity, $"Voided {number}: {reason.Trim()}");
                }

                using (var update = conn.CreateCommand())
                {
                    update.Transaction = tx;
                    update.CommandText = @"
UPDATE invoices SET status='VOID',void_reason=$reason,voided_at=$at,
 voided_by_user_id=$uid,voided_by_username=$user WHERE id=$id;";
                    update.Parameters.AddWithValue("$reason", reason.Trim());
                    update.Parameters.AddWithValue("$at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    update.Parameters.AddWithValue("$uid", AppSession.UserId);
                    update.Parameters.AddWithValue("$user", AppSession.Username);
                    update.Parameters.AddWithValue("$id", invoiceId);
                    update.ExecuteNonQuery();
                }
                WriteAudit(conn, tx, "VOID_INVOICE", "invoices", invoiceId, $"{number}; reason={reason.Trim()}");
                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        public void UpdateInvoiceDetails(InvoiceHeader invoice)
        {
            if (!AppSession.IsAdmin) throw new InvalidOperationException("Admin access is required to edit an invoice.");
            if (string.IsNullOrWhiteSpace(invoice.CustomerName)) throw new ArgumentException("Customer name is required.");
            using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                using var cmd = conn.CreateCommand(); cmd.Transaction = tx;
                cmd.CommandText = @"
UPDATE invoices SET customer_name=$name,customer_phone=$phone,customer_address=$address,
 payment_method=$method,payment_bank=$bank,notes=$notes WHERE id=$id;";
                cmd.Parameters.AddWithValue("$name", invoice.CustomerName.Trim()); cmd.Parameters.AddWithValue("$phone", Db(invoice.CustomerPhone));
                cmd.Parameters.AddWithValue("$address", Db(invoice.CustomerAddress)); cmd.Parameters.AddWithValue("$method", invoice.PaymentMethod.Trim().ToUpperInvariant());
                cmd.Parameters.AddWithValue("$bank", Db(invoice.PaymentBank)); cmd.Parameters.AddWithValue("$notes", Db(invoice.Notes)); cmd.Parameters.AddWithValue("$id", invoice.Id);
                if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Invoice not found.");
                WriteAudit(conn, tx, "UPDATE_INVOICE", "invoices", invoice.Id, $"{invoice.InvoiceNumber}; customer={invoice.CustomerName.Trim()}"); tx.Commit();
            }
            catch { try { tx.Rollback(); } catch { } throw; }
        }

        public void DeleteInvoiceRecord(int invoiceId, string reason)
        {
            if (!AppSession.IsAdmin) throw new InvalidOperationException("Admin access is required to delete an invoice record.");
            var invoice = GetInvoice(invoiceId);
            if (!invoice.Status.Equals("VOID", StringComparison.OrdinalIgnoreCase)) VoidInvoice(invoiceId, reason);
            using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                using (var detach = conn.CreateCommand())
                {
                    detach.Transaction = tx; detach.CommandText = "UPDATE stock_movements SET invoice_id=NULL WHERE invoice_id=$id;";
                    detach.Parameters.AddWithValue("$id", invoiceId); detach.ExecuteNonQuery();
                }
                using var cmd = conn.CreateCommand(); cmd.Transaction = tx;
                cmd.CommandText = "DELETE FROM invoices WHERE id=$id;"; cmd.Parameters.AddWithValue("$id", invoiceId);
                if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Invoice not found.");
                WriteAudit(conn, tx, "DELETE_INVOICE_RECORD", "invoices", invoiceId, $"{invoice.InvoiceNumber}; reason={reason}"); tx.Commit();
            }
            catch { try { tx.Rollback(); } catch { } throw; }
        }

        public DataTable GetInvoices(string? search = null)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT i.id,i.invoice_number,i.customer_name,i.customer_phone,i.payment_method,
       COALESCE(i.payment_bank,'') AS payment_bank,i.status,i.created_by_username,i.created_at,
       COALESCE(SUM(ii.line_total),0) AS total
FROM invoices i LEFT JOIN invoice_items ii ON ii.invoice_id=i.id
WHERE $q='' OR UPPER(i.invoice_number) LIKE $like OR UPPER(i.customer_name) LIKE $like
GROUP BY i.id ORDER BY datetime(i.created_at) DESC,i.id DESC;";
            var q = search?.Trim().ToUpperInvariant() ?? "";
            cmd.Parameters.AddWithValue("$q", q);
            cmd.Parameters.AddWithValue("$like", $"%{q}%");
            using var reader = cmd.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        public InvoiceHeader GetInvoice(int invoiceId)
        {
            using var conn = Database.OpenConnection();
            var result = new InvoiceHeader();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT id,invoice_number,customer_name,COALESCE(customer_phone,''),COALESCE(customer_address,''),
 payment_method,COALESCE(payment_bank,''),COALESCE(notes,''),status,COALESCE(created_by_username,''),created_at
FROM invoices WHERE id=$id;";
                cmd.Parameters.AddWithValue("$id", invoiceId);
                using var r = cmd.ExecuteReader();
                if (!r.Read()) throw new InvalidOperationException("Invoice not found.");
                result.Id = r.GetInt32(0);
                result.InvoiceNumber = r.GetString(1);
                result.CustomerName = r.GetString(2);
                result.CustomerPhone = r.GetString(3);
                result.CustomerAddress = r.GetString(4);
                result.PaymentMethod = r.GetString(5);
                result.PaymentBank = r.GetString(6);
                result.Notes = r.GetString(7);
                result.Status = r.GetString(8);
                result.CreatedBy = r.GetString(9);
                result.CreatedAt = DateTime.Parse(r.GetString(10), CultureInfo.InvariantCulture);
            }
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT product_id,brand,type,color,quantity,unit_price,COALESCE(frame_numbers,'')
FROM invoice_items WHERE invoice_id=$id ORDER BY id;";
                cmd.Parameters.AddWithValue("$id", invoiceId);
                using var r = cmd.ExecuteReader();
                while (r.Read()) result.Items.Add(new InvoiceDraftItem
                {
                    ProductId = r.GetInt32(0), Brand = r.GetString(1), Type = r.GetString(2),
                    Color = r.IsDBNull(3) ? null : r.GetString(3), Quantity = r.GetInt32(4),
                    UnitPrice = Convert.ToDecimal(r.GetDouble(5)), FrameNumbers = r.GetString(6)
                });
            }
            result.Total = result.Items.Sum(x => x.LineTotal);
            return result;
        }

        private static void InsertInvoiceItem(SqliteConnection conn, SqliteTransaction tx, long invoiceId,
            string customerName, DateTime now, InvoiceDraftItem item)
        {
            var before = GetAvailable(conn, tx, item.ProductId);
            long saleId;
            using (var sale = conn.CreateCommand())
            {
                sale.Transaction = tx;
                sale.CommandText = @"
INSERT INTO sales (brand,type,color,quantity,price,customer_name,date_time,voided,
 created_by_user_id,created_by_username,created_at)
VALUES ($brand,$type,$color,$qty,$price,$customer,$at,0,$uid,$user,$at);
SELECT last_insert_rowid();";
                sale.Parameters.AddWithValue("$brand", item.Brand);
                sale.Parameters.AddWithValue("$type", item.Type);
                sale.Parameters.AddWithValue("$color", Db(item.Color));
                sale.Parameters.AddWithValue("$qty", item.Quantity);
                sale.Parameters.AddWithValue("$price", (double)item.UnitPrice);
                sale.Parameters.AddWithValue("$customer", customerName);
                sale.Parameters.AddWithValue("$at", now.ToString("yyyy-MM-dd HH:mm:ss"));
                sale.Parameters.AddWithValue("$uid", AppSession.UserId);
                sale.Parameters.AddWithValue("$user", AppSession.Username);
                saleId = Convert.ToInt64(sale.ExecuteScalar() ?? 0L);
            }

            var remaining = item.Quantity;
            var lots = new List<(int Id, decimal Cost, int Qty)>();
            using (var getLots = conn.CreateCommand())
            {
                getLots.Transaction = tx;
                getLots.CommandText = @"
SELECT id,unit_cost,qty_remaining FROM stock_lots
WHERE product_id=$pid AND qty_remaining>0 ORDER BY datetime(received_at),id;";
                getLots.Parameters.AddWithValue("$pid", item.ProductId);
                using var reader = getLots.ExecuteReader();
                while (reader.Read()) lots.Add((reader.GetInt32(0), Convert.ToDecimal(reader.GetDouble(1)), reader.GetInt32(2)));
            }
            foreach (var lot in lots)
            {
                if (remaining == 0) break;
                var take = Math.Min(remaining, lot.Qty);
                using (var update = conn.CreateCommand())
                {
                    update.Transaction = tx;
                    update.CommandText = "UPDATE stock_lots SET qty_remaining=qty_remaining-$qty WHERE id=$id;";
                    update.Parameters.AddWithValue("$qty", take);
                    update.Parameters.AddWithValue("$id", lot.Id);
                    update.ExecuteNonQuery();
                }
                using (var line = conn.CreateCommand())
                {
                    line.Transaction = tx;
                    line.CommandText = @"
INSERT INTO sale_lines (sale_id,stock_lot_id,qty_sold,unit_cost,unit_sell,
 created_by_user_id,created_by_username,created_at)
VALUES ($sale,$lot,$qty,$cost,$sell,$uid,$user,$at);";
                    line.Parameters.AddWithValue("$sale", saleId);
                    line.Parameters.AddWithValue("$lot", lot.Id);
                    line.Parameters.AddWithValue("$qty", take);
                    line.Parameters.AddWithValue("$cost", (double)lot.Cost);
                    line.Parameters.AddWithValue("$sell", (double)item.UnitPrice);
                    line.Parameters.AddWithValue("$uid", AppSession.UserId);
                    line.Parameters.AddWithValue("$user", AppSession.Username);
                    line.Parameters.AddWithValue("$at", now.ToString("yyyy-MM-dd HH:mm:ss"));
                    line.ExecuteNonQuery();
                }
                remaining -= take;
            }
            if (remaining != 0) throw new InvalidOperationException($"FIFO allocation failed for {item.Bike}.");

            using (var invoiceItem = conn.CreateCommand())
            {
                invoiceItem.Transaction = tx;
                invoiceItem.CommandText = @"
INSERT INTO invoice_items
(invoice_id,sale_id,product_id,brand,type,color,quantity,unit_price,line_total,frame_numbers,created_at)
VALUES ($invoice,$sale,$product,$brand,$type,$color,$qty,$price,$total,$frames,$at);";
                invoiceItem.Parameters.AddWithValue("$invoice", invoiceId);
                invoiceItem.Parameters.AddWithValue("$sale", saleId);
                invoiceItem.Parameters.AddWithValue("$product", item.ProductId);
                invoiceItem.Parameters.AddWithValue("$brand", item.Brand);
                invoiceItem.Parameters.AddWithValue("$type", item.Type);
                invoiceItem.Parameters.AddWithValue("$color", Db(item.Color));
                invoiceItem.Parameters.AddWithValue("$qty", item.Quantity);
                invoiceItem.Parameters.AddWithValue("$price", (double)item.UnitPrice);
                invoiceItem.Parameters.AddWithValue("$total", (double)item.LineTotal);
                invoiceItem.Parameters.AddWithValue("$frames", Db(item.FrameNumbers));
                invoiceItem.Parameters.AddWithValue("$at", now.ToString("yyyy-MM-dd HH:mm:ss"));
                invoiceItem.ExecuteNonQuery();
            }
            InsertMovement(conn, tx, item.ProductId, null, invoiceId, "SALE", -item.Quantity,
                before, before - item.Quantity, $"Invoice sale: {item.Bike}");
        }

        private static int GetAvailable(SqliteConnection conn, SqliteTransaction tx, int productId)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "SELECT COALESCE(SUM(qty_remaining),0) FROM stock_lots WHERE product_id=$id;";
            cmd.Parameters.AddWithValue("$id", productId);
            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }

        private static string NextInvoiceNumber(SqliteConnection conn, SqliteTransaction tx, DateTime now)
        {
            var code = now.ToString("yyyyMMdd");
            using (var upsert = conn.CreateCommand())
            {
                upsert.Transaction = tx;
                upsert.CommandText = @"
INSERT INTO invoice_sequences(date_code,last_sequence,updated_at) VALUES ($code,1,$at)
ON CONFLICT(date_code) DO UPDATE SET last_sequence=last_sequence+1,updated_at=$at;";
                upsert.Parameters.AddWithValue("$code", code);
                upsert.Parameters.AddWithValue("$at", now.ToString("yyyy-MM-dd HH:mm:ss"));
                upsert.ExecuteNonQuery();
            }
            using var get = conn.CreateCommand();
            get.Transaction = tx;
            get.CommandText = "SELECT last_sequence FROM invoice_sequences WHERE date_code=$code;";
            get.Parameters.AddWithValue("$code", code);
            var sequence = Convert.ToInt32(get.ExecuteScalar() ?? 0);
            return $"INV-{code}-{sequence:000}";
        }

        internal static void InsertMovement(SqliteConnection conn, SqliteTransaction tx, int productId,
            int? lotId, long? invoiceId, string type, int change, int before, int after, string note)
        {
            string brand = "", bikeName = "", color = "", bikeId = "";
            using (var product = conn.CreateCommand())
            {
                product.Transaction = tx;
                product.CommandText = @"
SELECT p.brand,p.type,COALESCE(p.color,''),COALESCE((
 SELECT b.id FROM bikes b WHERE UPPER(b.brand)=UPPER(p.brand) AND UPPER(b.name)=UPPER(p.type) LIMIT 1
),'') FROM products p WHERE p.id=$id;";
                product.Parameters.AddWithValue("$id", productId);
                using var reader = product.ExecuteReader();
                if (reader.Read()) { brand=reader.GetString(0); bikeName=reader.GetString(1); color=reader.GetString(2); bikeId=reader.GetString(3); }
            }
            var websiteType = type.ToUpperInvariant() switch
            {
                "SALE" => "sale",
                "STOCK_IN" or "OPENING_STOCK" => "stock_in",
                _ => "adjustment"
            };
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
INSERT INTO stock_movements
(product_id,stock_lot_id,invoice_id,movement_type,quantity_change,quantity_before,quantity_after,
 note,created_by_user_id,created_by_username,created_at,bike_id,bike_brand,bike_name,bike_color_name,created_by_role)
VALUES ($product,$lot,$invoice,$type,$change,$before,$after,$note,$uid,$user,$at,$bike,$brand,$name,$color,$role);";
            cmd.Parameters.AddWithValue("$product", productId);
            cmd.Parameters.AddWithValue("$lot", (object?)lotId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$invoice", (object?)invoiceId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$type", websiteType);
            cmd.Parameters.AddWithValue("$change", change);
            cmd.Parameters.AddWithValue("$before", before);
            cmd.Parameters.AddWithValue("$after", after);
            cmd.Parameters.AddWithValue("$note", note);
            cmd.Parameters.AddWithValue("$uid", AppSession.UserId > 0 ? AppSession.UserId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$user", Db(AppSession.Username));
            cmd.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("o"));
            cmd.Parameters.AddWithValue("$bike", Db(bikeId));
            cmd.Parameters.AddWithValue("$brand", brand);
            cmd.Parameters.AddWithValue("$name", bikeName);
            cmd.Parameters.AddWithValue("$color", color);
            cmd.Parameters.AddWithValue("$role", Db(AppSession.Role));
            cmd.ExecuteNonQuery();
            WebsiteBikeRepository.RefreshBikeStockForProduct(conn, tx, productId);
        }

        internal static void WriteAudit(SqliteConnection conn, SqliteTransaction tx, string action,
            string entity, long? entityId, string detail)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
INSERT INTO audit_log(action,entity,entity_id,actor_user_id,actor_username,detail,created_at)
VALUES ($action,$entity,$id,$uid,$user,$detail,$at);";
            cmd.Parameters.AddWithValue("$action", action);
            cmd.Parameters.AddWithValue("$entity", entity);
            cmd.Parameters.AddWithValue("$id", (object?)entityId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$uid", AppSession.UserId > 0 ? AppSession.UserId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$user", Db(AppSession.Username));
            cmd.Parameters.AddWithValue("$detail", detail);
            cmd.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        private static object Db(string? value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
    }
}
