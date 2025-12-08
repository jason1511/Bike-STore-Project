using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp1;

namespace Bike_STore_Project
{
    public class ProductRepository
    {
        private SqliteConnection GetConnection() => Database.OpenConnection();

        public List<Product> GetAll(string? search = null)
        {
            var list = new List<Product>();

            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();

            if (string.IsNullOrWhiteSpace(search))
            {
                cmd.CommandText = "SELECT id, serial, model, price FROM products ORDER BY model;";
            }
            else
            {
                cmd.CommandText = @"
SELECT id, serial, model, price
FROM products
WHERE serial LIKE $q OR model LIKE $q
ORDER BY model;";
                cmd.Parameters.AddWithValue("$q", $"%{search.Trim()}%");
            }

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Product
                {
                    Id = rdr.GetInt32(0),
                    Serial = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1),
                    Model = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2),
                    Price = rdr.IsDBNull(3) ? 0m : Convert.ToDecimal(rdr.GetDouble(3))
                });
            }

            return list;
        }

        public int Insert(Product p)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO products (serial, model, price)
VALUES ($serial, $model, $price);
SELECT last_insert_rowid();
";
            cmd.Parameters.AddWithValue("$serial", p.Serial);
            cmd.Parameters.AddWithValue("$model", p.Model);
            cmd.Parameters.AddWithValue("$price", (double)p.Price);

            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }

        public bool Update(Product p)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
UPDATE products
SET serial = $serial,
    model  = $model,
    price  = $price
WHERE id = $id;";
            cmd.Parameters.AddWithValue("$serial", p.Serial);
            cmd.Parameters.AddWithValue("$model", p.Model);
            cmd.Parameters.AddWithValue("$price", (double)p.Price);
            cmd.Parameters.AddWithValue("$id", p.Id);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM products WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public Product? GetById(int id)
        {
            using var conn = GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, serial, model, price FROM products WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new Product
                {
                    Id = rdr.GetInt32(0),
                    Serial = rdr.GetString(1),
                    Model = rdr.GetString(2),
                    Price = Convert.ToDecimal(rdr.GetDouble(3))
                };
            }

            return null;
        }
    }
}
