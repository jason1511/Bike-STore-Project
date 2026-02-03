using System;
using Microsoft.Data.Sqlite;

namespace Bike_STore_Project
{
    public static class Database
    {
        // Default for the real app
        private static string _connectionString = "Data Source=data.db";

        /// <summary>
        /// Used by tests to redirect DB to a temporary file.
        /// Call this BEFORE Initialize().
        /// </summary>
        public static void UseDatabaseFile(string filePath)
        {
            _connectionString = $"Data Source={filePath}";
        }

        public static void Initialize()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS products (
    id       INTEGER PRIMARY KEY AUTOINCREMENT,
    brand    TEXT NOT NULL,
    type     TEXT NOT NULL,
    color    TEXT,
    quantity INTEGER NOT NULL DEFAULT 0,
    price    REAL NOT NULL DEFAULT 0.0,
    UNIQUE(brand, type, color)
);

CREATE TABLE IF NOT EXISTS sales (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    brand         TEXT NOT NULL,
    type          TEXT NOT NULL,
    color         TEXT,
    quantity      INTEGER NOT NULL DEFAULT 1,
    price         REAL NOT NULL DEFAULT 0.0, -- UNIT sell price (manual)
    customer_name TEXT,
    date_time     TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    voided        INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS services (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    brand         TEXT NOT NULL,
    type          TEXT NOT NULL,
    color         TEXT,
    quantity      INTEGER NOT NULL DEFAULT 1,
    service_cost  REAL NOT NULL DEFAULT 0.0,
    notes         TEXT,
    date_time     TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- batches / stock receipts (base cost per batch)
CREATE TABLE IF NOT EXISTS stock_lots (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    product_id    INTEGER NOT NULL,
    received_at   TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    unit_cost     REAL NOT NULL,
    qty_received  INTEGER NOT NULL,
    qty_remaining INTEGER NOT NULL,
    notes         TEXT,
    FOREIGN KEY(product_id) REFERENCES products(id) ON DELETE CASCADE
);

-- sale breakdown by lot (audit + profit)
CREATE TABLE IF NOT EXISTS sale_lines (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    sale_id       INTEGER NOT NULL,
    stock_lot_id  INTEGER NOT NULL,
    qty_sold      INTEGER NOT NULL,
    unit_cost     REAL NOT NULL,
    unit_sell     REAL NOT NULL,
    FOREIGN KEY(sale_id) REFERENCES sales(id) ON DELETE CASCADE,
    FOREIGN KEY(stock_lot_id) REFERENCES stock_lots(id)
);

CREATE INDEX IF NOT EXISTS idx_stock_lots_product_remaining
ON stock_lots(product_id, qty_remaining);

CREATE INDEX IF NOT EXISTS idx_stock_lots_received_at
ON stock_lots(product_id, received_at);

CREATE INDEX IF NOT EXISTS idx_sale_lines_sale_id
ON sale_lines(sale_id);

CREATE INDEX IF NOT EXISTS idx_sale_lines_lot_id
ON sale_lines(stock_lot_id);
";
                cmd.ExecuteNonQuery();
            }

            // One-time migration: convert existing products.quantity/price into initial stock lots
            using (var migrate = conn.CreateCommand())
            {
                migrate.CommandText = @"
INSERT INTO stock_lots (product_id, received_at, unit_cost, qty_received, qty_remaining, notes)
SELECT p.id, CURRENT_TIMESTAMP, p.price, p.quantity, p.quantity, 'Migrated from legacy products.quantity/price'
FROM products p
WHERE p.quantity > 0
  AND NOT EXISTS (SELECT 1 FROM stock_lots l WHERE l.product_id = p.id);
";
                migrate.ExecuteNonQuery();
            }

            // Ensure audit fields exist on sales (for migrated DBs)
            EnsureColumn(conn, "sales", "created_by_user_id", "INTEGER");
            EnsureColumn(conn, "sales", "created_by_username", "TEXT");
            EnsureColumn(conn, "sales", "created_at", "TEXT");

            // ✅ NEW: Ensure audit fields exist on sale_lines (for migrated DBs)
            EnsureColumn(conn, "sale_lines", "created_by_user_id", "INTEGER");
            EnsureColumn(conn, "sale_lines", "created_by_username", "TEXT");
            EnsureColumn(conn, "sale_lines", "created_at", "TEXT");
        }

        private static void EnsureColumn(SqliteConnection conn, string table, string column, string columnSqlType)
        {
            // Check if the column already exists
            using (var check = conn.CreateCommand())
            {
                check.CommandText = $"PRAGMA table_info({table});";
                using var rdr = check.ExecuteReader();
                while (rdr.Read())
                {
                    var name = rdr.GetString(1); // column name
                    if (string.Equals(name, column, StringComparison.OrdinalIgnoreCase))
                        return;
                }
            }

            // Add the column
            using (var alter = conn.CreateCommand())
            {
                alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {columnSqlType};";
                alter.ExecuteNonQuery();
            }
        }

        public static SqliteConnection OpenConnection()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
