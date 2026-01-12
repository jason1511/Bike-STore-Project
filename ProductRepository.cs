using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bike_STore_Project
{
    public class ProductRepository
    {
        public List<Product> GetAll(string? search = null)
        {
            var list = new List<Product>();
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();

            var where = "";
            if (!string.IsNullOrWhiteSpace(search))
            {
                where = @"WHERE p.brand LIKE $q OR p.type LIKE $q OR COALESCE(p.color,'') LIKE $q";
                cmd.Parameters.AddWithValue("$q", $"%{search.Trim().ToUpperInvariant()}%");
            }

            cmd.CommandText = $@"
SELECT
    p.id,
    p.brand,
    p.type,
    p.color,
    COALESCE(SUM(l.qty_remaining), 0) AS total_qty,
    COALESCE(MAX(l.unit_cost), 0) AS latest_cost,
    MAX(l.received_at) AS last_received_at
FROM products p
LEFT JOIN stock_lots l
    ON l.product_id = p.id
   AND l.qty_remaining > 0
{where}
GROUP BY p.id, p.brand, p.type, p.color
ORDER BY p.brand, p.type;";


            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Product
                {
                    Id = rdr.GetInt32(0),
                    Brand = rdr.GetString(1),
                    Type = rdr.GetString(2),
                    Color = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                    Quantity = rdr.GetInt32(4),
                    Price = Convert.ToDecimal(rdr.GetDouble(5)),
                    LastReceivedAt = rdr.IsDBNull(6)
        ? (DateTime?)null
        : DateTime.Parse(rdr.GetString(6))
                });

            }

            return list;
        }
        public bool TryMakeSaleFifoManualPrice(
            int productId,
            int saleQty,
            decimal saleUnitPrice,
            string? customerName,
            out string errorMessage)
        {
            errorMessage = "";

            if (saleQty <= 0)
            {
                errorMessage = "Quantity must be at least 1.";
                return false;
            }

            if (saleUnitPrice <= 0)
            {
                errorMessage = "Sale price must be greater than 0.";
                return false;
            }

            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                // Load product identity for sales table
                string brand, type;
                string? color;

                using (var prodCmd = conn.CreateCommand())
                {
                    prodCmd.Transaction = tx;
                    prodCmd.CommandText = @"SELECT brand, type, color FROM products WHERE id=$id;";
                    prodCmd.Parameters.AddWithValue("$id", productId);

                    using var rdr = prodCmd.ExecuteReader();
                    if (!rdr.Read())
                    {
                        errorMessage = "Product not found.";
                        tx.Rollback();
                        return false;
                    }

                    brand = rdr.GetString(0);
                    type = rdr.GetString(1);
                    color = rdr.IsDBNull(2) ? null : rdr.GetString(2);
                }

                // Check total available across lots
                int available;
                using (var availCmd = conn.CreateCommand())
                {
                    availCmd.Transaction = tx;
                    availCmd.CommandText = @"
SELECT COALESCE(SUM(qty_remaining),0)
FROM stock_lots
WHERE product_id=$id AND qty_remaining > 0;";
                    availCmd.Parameters.AddWithValue("$id", productId);
                    available = Convert.ToInt32(availCmd.ExecuteScalar() ?? 0);
                }

                if (available < saleQty)
                {
                    errorMessage = $"Not enough stock. Available: {available}.";
                    tx.Rollback();
                    return false;
                }

                // Insert sale header (price = UNIT sell price)
                long saleId;
                using (var saleCmd = conn.CreateCommand())
                {
                    saleCmd.Transaction = tx;
                    saleCmd.CommandText = @"
INSERT INTO sales (brand, type, color, quantity, price, customer_name)
VALUES ($brand, $type, $color, $qty, $price, $customer);
SELECT last_insert_rowid();";

                    saleCmd.Parameters.AddWithValue("$brand", brand);
                    saleCmd.Parameters.AddWithValue("$type", type);
                    saleCmd.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);
                    saleCmd.Parameters.AddWithValue("$qty", saleQty);
                    saleCmd.Parameters.AddWithValue("$price", (double)saleUnitPrice);
                    saleCmd.Parameters.AddWithValue("$customer", string.IsNullOrWhiteSpace(customerName) ? (object)DBNull.Value : customerName.Trim());

                    saleId = (long)(saleCmd.ExecuteScalar() ?? 0L);
                }

                // FIFO consume lots
                int remainingToSell = saleQty;

                using (var lotsCmd = conn.CreateCommand())
                {
                    lotsCmd.Transaction = tx;
                    lotsCmd.CommandText = @"
SELECT id, unit_cost, qty_remaining
FROM stock_lots
WHERE product_id=$pid AND qty_remaining > 0
ORDER BY received_at ASC, id ASC;";
                    lotsCmd.Parameters.AddWithValue("$pid", productId);

                    using var rdr = lotsCmd.ExecuteReader();
                    while (rdr.Read() && remainingToSell > 0)
                    {
                        var lotId = rdr.GetInt32(0);
                        var unitCost = Convert.ToDecimal(rdr.GetDouble(1));
                        var lotRemaining = rdr.GetInt32(2);

                        var take = Math.Min(lotRemaining, remainingToSell);
                        remainingToSell -= take;

                        // Deduct from lot
                        using (var updLot = conn.CreateCommand())
                        {
                            updLot.Transaction = tx;
                            updLot.CommandText = @"UPDATE stock_lots SET qty_remaining = qty_remaining - $take WHERE id=$id;";
                            updLot.Parameters.AddWithValue("$take", take);
                            updLot.Parameters.AddWithValue("$id", lotId);
                            updLot.ExecuteNonQuery();
                        }

                        // Record sale line with cost + sell snapshots
                        using (var lineCmd = conn.CreateCommand())
                        {
                            lineCmd.Transaction = tx;
                            lineCmd.CommandText = @"
INSERT INTO sale_lines (sale_id, stock_lot_id, qty_sold, unit_cost, unit_sell)
VALUES ($saleId, $lotId, $qty, $cost, $sell);";
                            lineCmd.Parameters.AddWithValue("$saleId", saleId);
                            lineCmd.Parameters.AddWithValue("$lotId", lotId);
                            lineCmd.Parameters.AddWithValue("$qty", take);
                            lineCmd.Parameters.AddWithValue("$cost", (double)unitCost);
                            lineCmd.Parameters.AddWithValue("$sell", (double)saleUnitPrice);
                            lineCmd.ExecuteNonQuery();
                        }
                    }
                }

                tx.Commit();
                return true;
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }
                errorMessage = "Sale failed: " + ex.Message;
                return false;
            }
        }

        public int AddProductIdentity(Product p)
        {
            var brand = p.Brand.Trim().ToUpperInvariant();
            var type = p.Type.Trim().ToUpperInvariant();
            var color = string.IsNullOrWhiteSpace(p.Color) ? null : p.Color.Trim().ToUpperInvariant();

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO products (brand, type, color, quantity, price)
VALUES ($brand, $type, $color, 0, 0.0);
SELECT last_insert_rowid();";

            cmd.Parameters.AddWithValue("$brand", brand);
            cmd.Parameters.AddWithValue("$type", type);
            cmd.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }
        public void ReceiveBatch(int productId, int qtyReceived, decimal unitCost, DateTime? receivedAt = null, string? notes = null)
        {
            if (qtyReceived <= 0) throw new ArgumentException("Quantity must be at least 1.");
            if (unitCost <= 0) throw new ArgumentException("Unit cost must be greater than 0.");

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO stock_lots (product_id, received_at, unit_cost, qty_received, qty_remaining, notes)
VALUES ($pid, $dt, $cost, $qty, $qty, $notes);";

            cmd.Parameters.AddWithValue("$pid", productId);
            cmd.Parameters.AddWithValue("$dt", (receivedAt ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$cost", (double)unitCost);
            cmd.Parameters.AddWithValue("$qty", qtyReceived);
            cmd.Parameters.AddWithValue("$notes", string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes.Trim());

            cmd.ExecuteNonQuery();
        }

        public int Insert(Product p)
        {
            var brand = p.Brand.Trim().ToUpperInvariant();
            var type = p.Type.Trim().ToUpperInvariant();
            var color = string.IsNullOrWhiteSpace(p.Color) ? null : p.Color.Trim().ToUpperInvariant();

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO products (brand, type, color, quantity, price)
VALUES ($brand, $type, $color, 0, 0.0);
SELECT last_insert_rowid();";

            cmd.Parameters.AddWithValue("$brand", brand);
            cmd.Parameters.AddWithValue("$type", type);
            cmd.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);

            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }
        public int GetOrCreateProductId(string brand, string type, string? color)
        {
            brand = brand.Trim().ToUpperInvariant();
            type = type.Trim().ToUpperInvariant();
            color = string.IsNullOrWhiteSpace(color) ? null : color.Trim().ToUpperInvariant();

            using var conn = Database.OpenConnection();

            // 1) Try find existing
            using (var find = conn.CreateCommand())
            {
                find.CommandText = @"
SELECT id
FROM products
WHERE brand = $brand
  AND type  = $type
  AND ((color IS NULL AND $color IS NULL) OR (color = $color))
LIMIT 1;";
                find.Parameters.AddWithValue("$brand", brand);
                find.Parameters.AddWithValue("$type", type);
                find.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);

                var existingId = find.ExecuteScalar();
                if (existingId != null && existingId != DBNull.Value)
                    return Convert.ToInt32(existingId);
            }

            // 2) Create new
            using (var ins = conn.CreateCommand())
            {
                ins.CommandText = @"
INSERT INTO products (brand, type, color, quantity, price)
VALUES ($brand, $type, $color, 0, 0.0);
SELECT last_insert_rowid();";
                ins.Parameters.AddWithValue("$brand", brand);
                ins.Parameters.AddWithValue("$type", type);
                ins.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);

                return Convert.ToInt32(ins.ExecuteScalar() ?? 0);
            }
        }

        public bool Update(Product p)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
UPDATE products
SET brand=$brand, type=$type, color=$color
WHERE id=$id;";

            cmd.Parameters.AddWithValue("$brand", p.Brand.Trim().ToUpperInvariant());
            cmd.Parameters.AddWithValue("$type", p.Type.Trim().ToUpperInvariant());
            cmd.Parameters.AddWithValue("$color", string.IsNullOrWhiteSpace(p.Color) ? (object)DBNull.Value : p.Color.Trim().ToUpperInvariant());
            cmd.Parameters.AddWithValue("$id", p.Id);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM products WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public Product? GetById(int id)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, brand, type, color, quantity, price
                                FROM products
                                WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return null;

            return new Product
            {
                Id = rdr.GetInt32(0),
                Brand = rdr.GetString(1),
                Type = rdr.GetString(2),
                Color = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                Quantity = rdr.GetInt32(4),
                Price = Convert.ToDecimal(rdr.GetDouble(5))
            };
        }
        public List<string> GetDistinctBrands()
        {
            var list = new List<string>();
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT brand FROM products ORDER BY brand;";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) list.Add(rdr.GetString(0));
            return list;
        }

        public List<string> GetDistinctTypes(string brand)
        {
            var list = new List<string>();
            brand = brand.Trim().ToUpperInvariant();

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT type FROM products WHERE brand = $brand ORDER BY type;";
            cmd.Parameters.AddWithValue("$brand", brand);

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) list.Add(rdr.GetString(0));
            return list;
        }

        public List<string> GetDistinctColors(string brand, string type)
        {
            var list = new List<string>();
            brand = brand.Trim().ToUpperInvariant();
            type = type.Trim().ToUpperInvariant();

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT DISTINCT COALESCE(color,'') AS color
FROM products
WHERE brand = $brand AND type = $type
ORDER BY color;";
            cmd.Parameters.AddWithValue("$brand", brand);
            cmd.Parameters.AddWithValue("$type", type);

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var c = rdr.GetString(0);
                if (!string.IsNullOrWhiteSpace(c))
                    list.Add(c);
            }
            return list;
        }
        public bool DeleteStockLot(int lotId)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM stock_lots WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", lotId);
            return cmd.ExecuteNonQuery() > 0;
        }
        public decimal? GetExistingPrice(string brand, string type, string? color)
        {
            brand = brand.Trim().ToUpperInvariant();
            type = type.Trim().ToUpperInvariant();
            color = string.IsNullOrWhiteSpace(color) ? null : color.Trim().ToUpperInvariant();

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT price
FROM products
WHERE brand = $brand
  AND type  = $type
  AND ((color IS NULL AND $color IS NULL) OR (color = $color))
LIMIT 1;";
            cmd.Parameters.AddWithValue("$brand", brand);
            cmd.Parameters.AddWithValue("$type", type);
            cmd.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);

            var result = cmd.ExecuteScalar();
            if (result == null || result == DBNull.Value) return null;

            return Convert.ToDecimal((double)result);
        }
        public List<StockLotRow> GetStockLots(string? search = null)
        {
            var list = new List<StockLotRow>();
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();

            var where = "";
            if (!string.IsNullOrWhiteSpace(search))
            {
                where = @"WHERE p.brand LIKE $q OR p.type LIKE $q OR COALESCE(p.color,'') LIKE $q OR COALESCE(l.notes,'') LIKE $q";
                cmd.Parameters.AddWithValue("$q", $"%{search.Trim().ToUpperInvariant()}%");
            }

            cmd.CommandText = $@"
SELECT
    l.id,
    l.product_id,
    p.brand,
    p.type,
    p.color,
    l.qty_received,
    l.qty_remaining,
    l.unit_cost,
    l.received_at,
    l.notes
FROM stock_lots l
JOIN products p ON p.id = l.product_id
{where}
ORDER BY datetime(l.received_at) DESC, l.id DESC;";

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new StockLotRow
                {
                    LotId = rdr.GetInt32(0),
                    ProductId = rdr.GetInt32(1),
                    Brand = rdr.GetString(2),
                    Type = rdr.GetString(3),
                    Color = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                    QtyReceived = rdr.GetInt32(5),
                    QtyRemaining = rdr.GetInt32(6),
                    UnitCost = Convert.ToDecimal(rdr.GetDouble(7)),
                    ReceivedAt = DateTime.Parse(rdr.GetString(8)),
                    Notes = rdr.IsDBNull(9) ? null : rdr.GetString(9)
                });
            }

            return list;
        }


        public bool UpdateStockLot(int lotId, int newQtyReceived, decimal newUnitCost)
        {
            using var conn = Database.OpenConnection();

            // Load current received/remaining to compute how many already sold
            int oldQtyReceived, oldQtyRemaining;
            using (var get = conn.CreateCommand())
            {
                get.CommandText = @"SELECT qty_received, qty_remaining FROM stock_lots WHERE id=$id;";
                get.Parameters.AddWithValue("$id", lotId);

                using var rdr = get.ExecuteReader();
                if (!rdr.Read()) return false;

                oldQtyReceived = rdr.GetInt32(0);
                oldQtyRemaining = rdr.GetInt32(1);
            }

            var sold = oldQtyReceived - oldQtyRemaining; // already consumed
            if (newQtyReceived < sold)
                throw new InvalidOperationException($"Cannot set received qty below already sold qty ({sold}).");

            var newRemaining = newQtyReceived - sold;

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
UPDATE stock_lots
SET qty_received = $received,
    qty_remaining = $remaining,
    unit_cost = $cost
WHERE id = $id;";
            cmd.Parameters.AddWithValue("$received", newQtyReceived);
            cmd.Parameters.AddWithValue("$remaining", newRemaining);
            cmd.Parameters.AddWithValue("$cost", (double)newUnitCost);
            cmd.Parameters.AddWithValue("$id", lotId);

            return cmd.ExecuteNonQuery() > 0;
        }


    }
}