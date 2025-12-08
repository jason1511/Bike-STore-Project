using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace WinFormsApp1
{
    public static class Database
    {
        private const string ConnectionString = "Data Source=data.db";

        public static void Initialize()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
CREATE TABLE IF NOT EXISTS sales (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    bike_model    TEXT NOT NULL,
    bike_serial   TEXT NOT NULL,
    price         REAL NOT NULL,
    customer_name TEXT,
    date_time     TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    voided        INTEGER DEFAULT 0
);

CREATE TABLE IF NOT EXISTS services (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    bike_serial   TEXT NOT NULL,
    service_type  TEXT,
    part_replaced TEXT,
    part_quantity INTEGER DEFAULT 0,
    service_cost  REAL,
    date_time     TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS products (
    id     INTEGER PRIMARY KEY AUTOINCREMENT,
    serial TEXT NOT NULL UNIQUE,
    model  TEXT NOT NULL,
    price  REAL NOT NULL
);
";
            command.ExecuteNonQuery();
        }

        public static SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}