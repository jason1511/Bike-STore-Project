using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS products (
    id       INTEGER PRIMARY KEY AUTOINCREMENT,
    brand    TEXT NOT NULL,
    type     TEXT NOT NULL,
    color    TEXT,
    quantity INTEGER NOT NULL DEFAULT 0,
    price    REAL NOT NULL DEFAULT 0.0
);

CREATE TABLE IF NOT EXISTS sales (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    brand         TEXT NOT NULL,
    type          TEXT NOT NULL,
    color         TEXT,
    quantity      INTEGER NOT NULL DEFAULT 1,
    price         REAL NOT NULL DEFAULT 0.0,
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
";
            cmd.ExecuteNonQuery();
        }

        public static SqliteConnection OpenConnection()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
