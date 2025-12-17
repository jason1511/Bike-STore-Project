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
                cmd.Parameters.AddWithValue("$q", $"%{search.Trim()}%");
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
    }
}