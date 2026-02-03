using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Bike_STore_Project
{
    public sealed class UserRepository
    {
        /// <summary>
        /// Call once at app startup. Creates users table + seeds admin if missing.
        /// </summary>
        public void EnsureUsersSchemaAndSeed()
        {
            using var conn = Database.OpenConnection();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS users (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  username TEXT NOT NULL UNIQUE,
  password_hash TEXT NOT NULL,
  role TEXT NOT NULL CHECK(role IN ('ADMIN','USER')),
  is_active INTEGER NOT NULL DEFAULT 1,
  created_at TEXT NOT NULL
);";
                cmd.ExecuteNonQuery();
            }

            // Seed default admin if none exists
            using (var check = conn.CreateCommand())
            {
                check.CommandText = @"SELECT COUNT(*) FROM users WHERE role='ADMIN';";
                var count = Convert.ToInt32(check.ExecuteScalar() ?? 0);

                if (count == 0)
                {
                    CreateUser("admin", "admin123", "ADMIN", isActive: true);
                }
            }
        }

        public int CreateUser(string username, string password, string role, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username required.");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password required.");

            role = role.Trim().ToUpperInvariant();
            if (role != "ADMIN" && role != "USER")
                throw new ArgumentException("Role must be ADMIN or USER.");

            username = username.Trim().ToLowerInvariant();
            var hash = PasswordHasher.Hash(password);

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO users (username, password_hash, role, is_active, created_at)
VALUES ($u, $h, $r, $a, $t);
SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$u", username);
            cmd.Parameters.AddWithValue("$h", hash);
            cmd.Parameters.AddWithValue("$r", role);
            cmd.Parameters.AddWithValue("$a", isActive ? 1 : 0);
            cmd.Parameters.AddWithValue("$t", DateTime.UtcNow.ToString("o"));

            return Convert.ToInt32((long)(cmd.ExecuteScalar() ?? 0L));
        }

        public bool TryLogin(string username, string password, out int userId, out string role, out string error)
        {
            userId = 0;
            role = "USER";
            error = "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                error = "Username and password are required.";
                return false;
            }

            username = username.Trim().ToLowerInvariant();

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT id, password_hash, role, is_active
FROM users
WHERE username = $u
LIMIT 1;";
            cmd.Parameters.AddWithValue("$u", username);

            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read())
            {
                error = "Invalid username or password.";
                return false;
            }

            var id = rdr.GetInt32(0);
            var storedHash = rdr.GetString(1);
            var dbRole = rdr.GetString(2);
            var isActive = rdr.GetInt32(3) == 1;

            if (!isActive)
            {
                error = "This user is disabled.";
                return false;
            }

            if (!PasswordHasher.Verify(password, storedHash))
            {
                error = "Invalid username or password.";
                return false;
            }

            userId = id;
            role = dbRole;
            return true;
        }

        // --------- User management API for UserManagementForm ---------

        public List<UserRow> GetUsers()
        {
            var list = new List<UserRow>();

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT id, username, role, is_active, created_at
FROM users
ORDER BY role DESC, username ASC;";

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new UserRow
                {
                    Id = rdr.GetInt32(0),
                    Username = rdr.GetString(1),
                    Role = rdr.GetString(2),
                    IsActive = rdr.GetInt32(3) == 1,
                    CreatedAt = DateTime.Parse(rdr.GetString(4))
                });
            }

            return list;
        }

        public void ResetPassword(int userId, string newPassword)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Password required.");

            var hash = PasswordHasher.Hash(newPassword);

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE users SET password_hash = $h WHERE id = $id;";
            cmd.Parameters.AddWithValue("$h", hash);
            cmd.Parameters.AddWithValue("$id", userId);

            if (cmd.ExecuteNonQuery() != 1)
                throw new InvalidOperationException("User not found.");
        }

        public void SetActive(int userId, bool isActive)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE users SET is_active = $a WHERE id = $id;";
            cmd.Parameters.AddWithValue("$a", isActive ? 1 : 0);
            cmd.Parameters.AddWithValue("$id", userId);
            cmd.ExecuteNonQuery();
        }

        public void SetRole(int userId, string role)
        {
            role = role.Trim().ToUpperInvariant();
            if (role != "ADMIN" && role != "USER")
                throw new ArgumentException("Role must be ADMIN or USER.");

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE users SET role = $r WHERE id = $id;";
            cmd.Parameters.AddWithValue("$r", role);
            cmd.Parameters.AddWithValue("$id", userId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int userId)
        {
            // safety: don’t delete yourself
            if (userId == AppSession.UserId)
                throw new InvalidOperationException("You cannot delete the currently signed-in user.");

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM users WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", userId);
            cmd.ExecuteNonQuery();
        }

        // --------- Backward compatibility (optional) ---------
        // If you still call ChangePassword somewhere, keep it:
        public void ChangePassword(int userId, string newPassword) => ResetPassword(userId, newPassword);

        // If you still call ListUsers somewhere, keep it:
        public List<(int Id, string Username, string Role, bool IsActive, DateTime CreatedAt)> ListUsers()
        {
            var rows = GetUsers();
            var list = new List<(int, string, string, bool, DateTime)>();
            foreach (var r in rows)
                list.Add((r.Id, r.Username, r.Role, r.IsActive, r.CreatedAt));
            return list;
        }
    }
}
