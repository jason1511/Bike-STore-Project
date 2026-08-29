using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Bike_STore_Project
{
    public sealed class UserRepository
    {
        public string? EnsureUsersSchemaAndSeed()
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
                    var initialPassword = GenerateInitialPassword();
                    CreateUser("admin", initialPassword, "ADMIN", isActive: true);
                    return initialPassword;
                }
            }

            return null;
        }

        private static string GenerateInitialPassword()
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
            var bytes = RandomNumberGenerator.GetBytes(16);
            var password = new char[16];
            for (var i = 0; i < password.Length; i++)
                password[i] = alphabet[bytes[i] % alphabet.Length];
            return new string(password);
        }

        public string RegenerateLocalAdminPassword()
        {
            var password = GenerateInitialPassword();
            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();
            try
            {
                int adminId;
                using (var find = conn.CreateCommand())
                {
                    find.Transaction = tx;
                    find.CommandText = "SELECT id FROM users WHERE username='admin' AND role='ADMIN' LIMIT 1;";
                    adminId = Convert.ToInt32(find.ExecuteScalar() ?? 0);
                }
                if (adminId == 0) throw new InvalidOperationException(Strings.Get("Error_UserNotFound"));

                using (var update = conn.CreateCommand())
                {
                    update.Transaction = tx;
                    update.CommandText = "UPDATE users SET password_hash=$hash,is_active=1 WHERE id=$id;";
                    update.Parameters.AddWithValue("$hash", PasswordHasher.Hash(password));
                    update.Parameters.AddWithValue("$id", adminId);
                    update.ExecuteNonQuery();
                }
                WriteAudit(conn, tx, "RECOVER_ADMIN_PASSWORD", "users", adminId, "username=admin; account_reactivated=1");
                tx.Commit();
                return password;
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        // ---------- AUDIT HELPERS ----------

        private static void WriteAudit(
            SqliteConnection conn,
            SqliteTransaction? tx,
            string action,
            string entity,
            int? entityId,
            string? detail)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;

            cmd.CommandText = @"
INSERT INTO audit_log (action, entity, entity_id, actor_user_id, actor_username, detail, created_at)
VALUES ($action, $entity, $entityId, $actorId, $actorUser, $detail, $at);";

            cmd.Parameters.AddWithValue("$action", action);
            cmd.Parameters.AddWithValue("$entity", entity);
            cmd.Parameters.AddWithValue("$entityId", (object?)entityId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$actorId", AppSession.UserId > 0 ? AppSession.UserId : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("$actorUser", string.IsNullOrWhiteSpace(AppSession.Username) ? (object)DBNull.Value : AppSession.Username);
            cmd.Parameters.AddWithValue("$detail", string.IsNullOrWhiteSpace(detail) ? (object)DBNull.Value : detail);
            cmd.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("o")); // consistent UTC

            cmd.ExecuteNonQuery();
        }

        private static (string Username, string Role, bool IsActive) GetUserSnapshot(SqliteConnection conn, SqliteTransaction? tx, int userId)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"SELECT username, role, is_active FROM users WHERE id=$id LIMIT 1;";
            cmd.Parameters.AddWithValue("$id", userId);

            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) throw new InvalidOperationException(Strings.Get("Error_UserNotFound"));

            return (
                rdr.GetString(0),
                rdr.GetString(1),
                rdr.GetInt32(2) == 1
            );
        }

        // ---------- LOGIN (RESTORED) ----------

        public bool TryLogin(string username, string password, out int userId, out string role, out string error)
        {
            userId = 0;
            role = "USER";
            error = "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                error = Strings.Get("Error_CredentialsRequired");
                return false;
            }

            username = username.Trim().ToLowerInvariant();

            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
SELECT id, password_hash, role, is_active
FROM users
WHERE username = $u
LIMIT 1;";
                cmd.Parameters.AddWithValue("$u", username);

                using var rdr = cmd.ExecuteReader();
                if (!rdr.Read())
                {
                    // optional audit
                    WriteAudit(conn, tx, "LOGIN_FAIL", "users", null, $"username={username}, reason=not_found");
                    tx.Commit();

                    error = Strings.Get("Error_InvalidCredentials");
                    return false;
                }

                var id = rdr.GetInt32(0);
                var storedHash = rdr.GetString(1);
                var dbRole = rdr.GetString(2);
                var isActive = rdr.GetInt32(3) == 1;

                if (!isActive)
                {
                    WriteAudit(conn, tx, "LOGIN_FAIL", "users", id, $"username={username}, reason=disabled");
                    tx.Commit();

                    error = Strings.Get("Error_UserDisabled");
                    return false;
                }

                if (!PasswordHasher.Verify(password, storedHash))
                {
                    WriteAudit(conn, tx, "LOGIN_FAIL", "users", id, $"username={username}, reason=bad_password");
                    tx.Commit();

                    error = Strings.Get("Error_InvalidCredentials");
                    return false;
                }

                // ✅ success
                userId = id;
                role = dbRole;

                WriteAudit(conn, tx, "LOGIN_SUCCESS", "users", id, $"username={username}, role={dbRole}");
                tx.Commit();

                return true;
            }
            catch (Exception ex)
            {
                try { tx.Rollback(); } catch { }
                error = Strings.Format("Error_LoginFailedDetail", ex.Message);
                return false;
            }
        }

        // ---------- CORE METHODS ----------

        public int CreateUser(string username, string password, string role, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException(Strings.Get("Error_UsernameRequired"));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException(Strings.Get("Error_PasswordRequired"));
            role = role.Trim().ToUpperInvariant();
            if (role != "ADMIN" && role != "USER") throw new ArgumentException(Strings.Get("Error_RoleCode"));

            username = username.Trim().ToLowerInvariant();
            var hash = PasswordHasher.Hash(password);

            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
INSERT INTO users (username, password_hash, role, is_active, created_at)
VALUES ($u, $h, $r, $a, $t);
SELECT last_insert_rowid();";
                    cmd.Parameters.AddWithValue("$u", username);
                    cmd.Parameters.AddWithValue("$h", hash);
                    cmd.Parameters.AddWithValue("$r", role);
                    cmd.Parameters.AddWithValue("$a", isActive ? 1 : 0);
                    cmd.Parameters.AddWithValue("$t", DateTime.UtcNow.ToString("o"));

                    var newId = Convert.ToInt32((long)(cmd.ExecuteScalar() ?? 0L));

                    WriteAudit(conn, tx, "CREATE_USER", "users", newId,
                        $"username={username}, role={role}, active={(isActive ? 1 : 0)}");

                    tx.Commit();
                    return newId;
                }
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        public void ResetPassword(int userId, string newPassword)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException(Strings.Get("Error_PasswordRequired"));

            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                var snap = GetUserSnapshot(conn, tx, userId);

                var hash = PasswordHasher.Hash(newPassword);
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"UPDATE users SET password_hash = $h WHERE id = $id;";
                    cmd.Parameters.AddWithValue("$h", hash);
                    cmd.Parameters.AddWithValue("$id", userId);

                    if (cmd.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(Strings.Get("Error_UserNotFound"));
                }

                WriteAudit(conn, tx, "RESET_PASSWORD", "users", userId, $"username={snap.Username}");
                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        public void SetActive(int userId, bool isActive)
        {
            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                var before = GetUserSnapshot(conn, tx, userId);

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"UPDATE users SET is_active = $a WHERE id = $id;";
                    cmd.Parameters.AddWithValue("$a", isActive ? 1 : 0);
                    cmd.Parameters.AddWithValue("$id", userId);

                    if (cmd.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(Strings.Get("Error_UserNotFound"));
                }

                WriteAudit(conn, tx, "TOGGLE_ACTIVE", "users", userId,
                    $"username={before.Username}, from_active={(before.IsActive ? 1 : 0)} to_active={(isActive ? 1 : 0)}");

                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        public void SetRole(int userId, string newRole)
        {
            newRole = (newRole ?? "").Trim().ToUpperInvariant();
            if (newRole != "ADMIN" && newRole != "USER")
                throw new ArgumentException(Strings.Get("Error_RoleCode"));

            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                var before = GetUserSnapshot(conn, tx, userId);

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"UPDATE users SET role = $r WHERE id = $id;";
                    cmd.Parameters.AddWithValue("$r", newRole);
                    cmd.Parameters.AddWithValue("$id", userId);

                    if (cmd.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(Strings.Get("Error_UserNotFound"));
                }

                WriteAudit(conn, tx, "SET_ROLE", "users", userId,
                    $"username={before.Username}, from_role={before.Role} to_role={newRole}");

                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

        public void DeleteUser(int userId)
        {
            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                var before = GetUserSnapshot(conn, tx, userId);

                WriteAudit(conn, tx, "DELETE_USER", "users", userId,
                    $"username={before.Username}, role={before.Role}, active={(before.IsActive ? 1 : 0)}");

                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"DELETE FROM users WHERE id = $id;";
                    cmd.Parameters.AddWithValue("$id", userId);

                    if (cmd.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException(Strings.Get("Error_UserNotFound"));
                }

                tx.Commit();
            }
            catch
            {
                try { tx.Rollback(); } catch { }
                throw;
            }
        }

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
                var createdText = rdr.GetString(4);
                DateTime? createdAt = DateTime.TryParse(createdText, out var parsed) ? parsed : null;
                list.Add(new UserRow
                {
                    Id = rdr.GetInt32(0),
                    Username = rdr.GetString(1),
                    Role = rdr.GetString(2),
                    IsActive = rdr.GetInt32(3) == 1,
                    CreatedAt = createdAt
                });
            }

            return list;
        }
    }
}
