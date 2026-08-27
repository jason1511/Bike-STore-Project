using System;
using Microsoft.Data.Sqlite;
using System.IO;

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
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Database path is required.", nameof(filePath));
            var fullPath = Path.GetFullPath(filePath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
            _connectionString = new SqliteConnectionStringBuilder { DataSource = fullPath }.ToString();
        }

        public static void DeleteDatabaseFile(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);
            SqliteConnection.ClearAllPools();
            if (File.Exists(fullPath)) File.Delete(fullPath);
            foreach (var suffix in new[] { "-wal", "-shm" })
            {
                var sidecar = fullPath + suffix;
                if (File.Exists(sidecar)) File.Delete(sidecar);
            }
        }

        public static void ValidateDatabaseFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Database path is required.", nameof(filePath));
            var fullPath = Path.GetFullPath(filePath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
            using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = fullPath }.ToString());
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA schema_version;";
            command.ExecuteScalar();
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
CREATE TABLE IF NOT EXISTS audit_log (
    id              INTEGER PRIMARY KEY AUTOINCREMENT,
    action          TEXT NOT NULL,
    entity          TEXT NOT NULL,
    entity_id        INTEGER,
    actor_user_id    INTEGER,
    actor_username   TEXT,
    detail           TEXT,
    created_at       TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_audit_log_entity
ON audit_log(entity, entity_id);

CREATE INDEX IF NOT EXISTS idx_audit_log_created_at
ON audit_log(created_at);

CREATE INDEX IF NOT EXISTS idx_audit_log_actor
ON audit_log(actor_user_id);

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

            // Local desktop equivalents of the website admin data model.
            // These tables are additive so existing desktop databases continue to work.
            using (var adminSchema = conn.CreateCommand())
            {
                adminSchema.CommandText = @"
CREATE TABLE IF NOT EXISTS brands (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT NOT NULL UNIQUE COLLATE NOCASE,
    is_active   INTEGER NOT NULL DEFAULT 1,
    sort_order  INTEGER NOT NULL DEFAULT 0,
    created_at  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Local SQLite copy of the website admin catalogue model. The JSON colours
-- payload deliberately matches the Cloudflare D1 `bikes.colors` shape.
CREATE TABLE IF NOT EXISTS bikes (
    id          TEXT PRIMARY KEY,
    brand_id    TEXT NOT NULL DEFAULT '',
    brand       TEXT NOT NULL,
    name        TEXT NOT NULL,
    battery     TEXT DEFAULT '',
    motor       TEXT DEFAULT '',
    topSpeed    TEXT DEFAULT '',
    range       TEXT DEFAULT '',
    maxWeight   TEXT DEFAULT '',
    safety      TEXT DEFAULT '',
    image       TEXT DEFAULT '',
    alt         TEXT DEFAULT '',
    comfort     TEXT DEFAULT 'medium',
    themeColor  TEXT DEFAULT '',
    themeColorSecond TEXT DEFAULT '',
    colorName   TEXT DEFAULT '',
    colors      TEXT NOT NULL DEFAULT '[]',
    description TEXT DEFAULT '',
    price       REAL DEFAULT 0,
    featured    INTEGER DEFAULT 0,
    inStock     INTEGER DEFAULT 1,
    stockQty    INTEGER DEFAULT 0,
    createdAt   TEXT DEFAULT CURRENT_TIMESTAMP,
    updatedAt   TEXT DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_bikes_brand ON bikes(brand);
CREATE INDEX IF NOT EXISTS idx_bikes_brand_id ON bikes(brand_id);
CREATE INDEX IF NOT EXISTS idx_bikes_stock ON bikes(inStock, stockQty);
CREATE INDEX IF NOT EXISTS idx_bikes_featured ON bikes(featured);

CREATE TABLE IF NOT EXISTS invoice_sequences (
    date_code     TEXT PRIMARY KEY,
    last_sequence INTEGER NOT NULL DEFAULT 0,
    updated_at    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS invoices (
    id                  INTEGER PRIMARY KEY AUTOINCREMENT,
    invoice_number      TEXT NOT NULL UNIQUE,
    customer_name       TEXT NOT NULL,
    customer_phone      TEXT,
    customer_address    TEXT,
    payment_method      TEXT NOT NULL DEFAULT 'CASH',
    payment_bank        TEXT,
    notes               TEXT,
    status              TEXT NOT NULL DEFAULT 'ACTIVE' CHECK(status IN ('ACTIVE','VOID')),
    void_reason         TEXT,
    voided_at           TEXT,
    voided_by_user_id   INTEGER,
    voided_by_username  TEXT,
    created_by_user_id  INTEGER,
    created_by_username TEXT,
    created_at          TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS invoice_items (
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    invoice_id     INTEGER NOT NULL,
    sale_id        INTEGER,
    product_id     INTEGER NOT NULL,
    brand          TEXT NOT NULL,
    type           TEXT NOT NULL,
    color          TEXT,
    quantity       INTEGER NOT NULL,
    unit_price     REAL NOT NULL,
    line_total     REAL NOT NULL,
    frame_numbers  TEXT,
    created_at     TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(invoice_id) REFERENCES invoices(id) ON DELETE CASCADE,
    FOREIGN KEY(product_id) REFERENCES products(id),
    FOREIGN KEY(sale_id) REFERENCES sales(id)
);

CREATE TABLE IF NOT EXISTS stock_movements (
    id                  INTEGER PRIMARY KEY AUTOINCREMENT,
    product_id          INTEGER NOT NULL,
    stock_lot_id        INTEGER,
    invoice_id          INTEGER,
    movement_type       TEXT NOT NULL,
    quantity_change     INTEGER NOT NULL,
    quantity_before     INTEGER NOT NULL,
    quantity_after      INTEGER NOT NULL,
    note                TEXT,
    created_by_user_id  INTEGER,
    created_by_username TEXT,
    created_at          TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(product_id) REFERENCES products(id),
    FOREIGN KEY(stock_lot_id) REFERENCES stock_lots(id),
    FOREIGN KEY(invoice_id) REFERENCES invoices(id)
);

CREATE INDEX IF NOT EXISTS idx_invoice_items_invoice ON invoice_items(invoice_id);
CREATE INDEX IF NOT EXISTS idx_invoices_created_at ON invoices(created_at);
CREATE INDEX IF NOT EXISTS idx_invoices_status ON invoices(status);
CREATE INDEX IF NOT EXISTS idx_stock_movements_product ON stock_movements(product_id);
CREATE INDEX IF NOT EXISTS idx_stock_movements_created_at ON stock_movements(created_at);
";
                adminSchema.ExecuteNonQuery();
            }

            EnsureColumn(conn, "services", "service_number", "TEXT");
            EnsureColumn(conn, "services", "customer_name", "TEXT");
            EnsureColumn(conn, "services", "customer_phone", "TEXT");
            EnsureColumn(conn, "services", "customer_address", "TEXT");
            EnsureColumn(conn, "services", "service_type", "TEXT");
            EnsureColumn(conn, "services", "service_status", "TEXT NOT NULL DEFAULT 'RECEIVED'");
            EnsureColumn(conn, "services", "completed_at", "TEXT");
            EnsureColumn(conn, "products", "battery", "TEXT");
            EnsureColumn(conn, "products", "motor", "TEXT");
            EnsureColumn(conn, "products", "top_speed", "TEXT");
            EnsureColumn(conn, "products", "range_text", "TEXT");
            EnsureColumn(conn, "products", "max_weight", "TEXT");
            EnsureColumn(conn, "products", "safety", "TEXT");
            EnsureColumn(conn, "products", "image_path", "TEXT");
            EnsureColumn(conn, "products", "description", "TEXT");
            EnsureColumn(conn, "products", "featured", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn(conn, "products", "is_active", "INTEGER NOT NULL DEFAULT 1");
            EnsureColumn(conn, "products", "sell_price", "REAL NOT NULL DEFAULT 0");
            EnsureColumn(conn, "stock_movements", "bike_id", "TEXT");
            EnsureColumn(conn, "stock_movements", "bike_brand", "TEXT");
            EnsureColumn(conn, "stock_movements", "bike_name", "TEXT");
            EnsureColumn(conn, "stock_movements", "bike_color_name", "TEXT NOT NULL DEFAULT ''");
            EnsureColumn(conn, "stock_movements", "created_by_role", "TEXT");

            using (var seedBrands = conn.CreateCommand())
            {
                seedBrands.CommandText = @"
INSERT OR IGNORE INTO brands(name, sort_order)
SELECT DISTINCT UPPER(TRIM(brand)), 0
FROM products
WHERE TRIM(COALESCE(brand,'')) <> '';
";
                seedBrands.ExecuteNonQuery();
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

            // Establish an idempotent opening movement history for pre-existing lots.
            using (var openingMovements = conn.CreateCommand())
            {
                openingMovements.CommandText = @"
INSERT INTO stock_movements
(product_id, stock_lot_id, movement_type, quantity_change, quantity_before, quantity_after,
 note, created_by_username, created_at)
SELECT product_id, id, 'OPENING_STOCK', qty_remaining,
       SUM(qty_remaining) OVER (PARTITION BY product_id ORDER BY datetime(received_at), id) - qty_remaining,
       SUM(qty_remaining) OVER (PARTITION BY product_id ORDER BY datetime(received_at), id),
       'Opening balance captured during local admin upgrade', 'SYSTEM', CURRENT_TIMESTAMP
FROM stock_lots l
WHERE NOT EXISTS (SELECT 1 FROM stock_movements sm WHERE sm.stock_lot_id=l.id);
";
                openingMovements.ExecuteNonQuery();
            }

            // Build the website-compatible bike/colour JSON model after legacy
            // quantities have been converted into FIFO lots.
            WebsiteBikeRepository.MigrateLegacyProducts(conn);

            // Ensure audit fields exist on sales (for migrated DBs)
            EnsureColumn(conn, "sales", "created_by_user_id", "INTEGER");
            EnsureColumn(conn, "sales", "created_by_username", "TEXT");
            EnsureColumn(conn, "sales", "created_at", "TEXT");

            // ✅ NEW: Ensure audit fields exist on sale_lines (for migrated DBs)
            EnsureColumn(conn, "sale_lines", "created_by_user_id", "INTEGER");
            EnsureColumn(conn, "sale_lines", "created_by_username", "TEXT");
            EnsureColumn(conn, "sale_lines", "created_at", "TEXT");
            EnsureColumn(conn, "services", "created_by_user_id", "INTEGER");
            EnsureColumn(conn, "services", "created_by_username", "TEXT");
            EnsureColumn(conn, "services", "created_at", "TEXT");

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
            using var pragma = conn.CreateCommand();
            pragma.CommandText = "PRAGMA foreign_keys = ON;";
            pragma.ExecuteNonQuery();
            return conn;
        }
    }
}
