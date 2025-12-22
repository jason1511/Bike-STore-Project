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

            if (string.IsNullOrWhiteSpace(search))
            {
                cmd.CommandText = @"SELECT id, brand, type, color, quantity, price
                                    FROM products
                                    ORDER BY brand, type;";
            }
            else
            {
                cmd.CommandText = @"SELECT id, brand, type, color, quantity, price
                                    FROM products
                                    WHERE brand LIKE $q OR type LIKE $q OR color LIKE $q
                                    ORDER BY brand, type;";
                cmd.Parameters.AddWithValue("$q", $"%{search.Trim().ToUpperInvariant()}%");

            }

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
                    Price = Convert.ToDecimal(rdr.GetDouble(5))
                });
            }

            return list;
        }

        public int Insert(Product p)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO products (brand, type, color, quantity, price)
VALUES ($brand, $type, $color, $qty, $price);
SELECT last_insert_rowid();";

            cmd.Parameters.AddWithValue("$brand", p.Brand.Trim());
            cmd.Parameters.AddWithValue("$type", p.Type.Trim());
            cmd.Parameters.AddWithValue("$color", string.IsNullOrWhiteSpace(p.Color) ? (object)DBNull.Value : p.Color.Trim());
            cmd.Parameters.AddWithValue("$qty", p.Quantity);
            cmd.Parameters.AddWithValue("$price", (double)p.Price);

            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }

        public bool Update(Product p)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
UPDATE products
SET brand=$brand, type=$type, color=$color, quantity=$qty, price=$price
WHERE id=$id;";

            cmd.Parameters.AddWithValue("$brand", p.Brand.Trim());
            cmd.Parameters.AddWithValue("$type", p.Type.Trim());
            cmd.Parameters.AddWithValue("$color", string.IsNullOrWhiteSpace(p.Color) ? (object)DBNull.Value : p.Color.Trim());
            cmd.Parameters.AddWithValue("$qty", p.Quantity);
            cmd.Parameters.AddWithValue("$price", (double)p.Price);
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

        public void AddOrIncreaseStock(Product p)
        {
            var brand = p.Brand.Trim().ToUpperInvariant();
            var type = p.Type.Trim().ToUpperInvariant();
            var color = string.IsNullOrWhiteSpace(p.Color) ? null : p.Color.Trim().ToUpperInvariant();

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();

            // Try update first (works even without UNIQUE constraint)
            cmd.CommandText = @"
UPDATE products
SET quantity = quantity + $addQty,
    price = $price
WHERE brand = $brand
  AND type  = $type
  AND ((color IS NULL AND $color IS NULL) OR (color = $color));";

            cmd.Parameters.AddWithValue("$addQty", p.Quantity);
            cmd.Parameters.AddWithValue("$price", (double)p.Price);
            cmd.Parameters.AddWithValue("$brand", brand);
            cmd.Parameters.AddWithValue("$type", type);
            cmd.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);

            var updated = cmd.ExecuteNonQuery();

            if (updated > 0) return;

            // If no row updated, insert new
            using var ins = conn.CreateCommand();
            ins.CommandText = @"
INSERT INTO products (brand, type, color, quantity, price)
VALUES ($brand, $type, $color, $qty, $price);";

            ins.Parameters.AddWithValue("$brand", brand);
            ins.Parameters.AddWithValue("$type", type);
            ins.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);
            ins.Parameters.AddWithValue("$qty", p.Quantity);
            ins.Parameters.AddWithValue("$price", (double)p.Price);

            ins.ExecuteNonQuery();
        }

        public bool TryMakeSale(
    string brand,
    string type,
    string? color,
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

            brand = brand.Trim();
            type = type.Trim();
            color = string.IsNullOrWhiteSpace(color) ? null : color.Trim();

            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                // Find matching product row
                using var findCmd = conn.CreateCommand();
                findCmd.Transaction = tx;
                findCmd.CommandText = @"
SELECT id, quantity, price
FROM products
WHERE brand = $brand
  AND type  = $type
  AND (
        (color IS NULL AND $color IS NULL)
        OR (color = $color)
      )
LIMIT 1;";
                findCmd.Parameters.AddWithValue("$brand", brand);
                findCmd.Parameters.AddWithValue("$type", type);
                findCmd.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);

                int productId;
                int currentQty;
                decimal inventoryPrice;

                using (var rdr = findCmd.ExecuteReader())
                {
                    if (!rdr.Read())
                    {
                        errorMessage = "No stock found for that Brand/Type/Color.";
                        tx.Rollback();
                        return false;
                    }

                    productId = rdr.GetInt32(0);
                    currentQty = rdr.GetInt32(1);
                    inventoryPrice = Convert.ToDecimal(rdr.GetDouble(2));
                }

                // Block sale if not enough stock
                if (currentQty < saleQty)
                {
                    errorMessage = $"Not enough stock. Available: {currentQty}.";
                    tx.Rollback();
                    return false;
                }

                // Optional: if user leaves price 0, default to inventory price
                if (saleUnitPrice <= 0)
                    saleUnitPrice = inventoryPrice;

                var newQty = currentQty - saleQty;

                // Update or delete stock
                if (newQty == 0)
                {
                    using var delCmd = conn.CreateCommand();
                    delCmd.Transaction = tx;
                    delCmd.CommandText = "DELETE FROM products WHERE id = $id;";
                    delCmd.Parameters.AddWithValue("$id", productId);
                    delCmd.ExecuteNonQuery();
                }
                else
                {
                    using var updCmd = conn.CreateCommand();
                    updCmd.Transaction = tx;
                    updCmd.CommandText = "UPDATE products SET quantity = $qty WHERE id = $id;";
                    updCmd.Parameters.AddWithValue("$qty", newQty);
                    updCmd.Parameters.AddWithValue("$id", productId);
                    updCmd.ExecuteNonQuery();
                }

                // Insert sale record
                using var saleCmd = conn.CreateCommand();
                saleCmd.Transaction = tx;
                saleCmd.CommandText = @"
INSERT INTO sales (brand, type, color, quantity, price, customer_name)
VALUES ($brand, $type, $color, $qty, $price, $customer);";
                saleCmd.Parameters.AddWithValue("$brand", brand);
                saleCmd.Parameters.AddWithValue("$type", type);
                saleCmd.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);
                saleCmd.Parameters.AddWithValue("$qty", saleQty);
                saleCmd.Parameters.AddWithValue("$price", (double)saleUnitPrice);
                saleCmd.Parameters.AddWithValue("$customer", string.IsNullOrWhiteSpace(customerName) ? (object)DBNull.Value : customerName.Trim());
                saleCmd.ExecuteNonQuery();

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

    }
}