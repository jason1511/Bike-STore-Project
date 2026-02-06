using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class UserManagementForm : Form
    {
        private readonly UserRepository _repo = new();
        private BindingList<UserRow> _bind = new();

        public UserManagementForm()
        {
            InitializeComponent();

            // ✅ Admin-only guard
            if (!AppSession.IsAdmin)
            {
                MessageBox.Show("Access denied. Admin only.",
                    "Permission denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Make sure form doesn't keep showing
                Shown += (_, __) => Close();
                return;
            }

            // Grid setup + events
            SetupGrid();

            Load += (_, __) => Reload();

            btnAddUser.Click += (_, __) => AddUser();
            btnResetPassword.Click += (_, __) => ResetPassword();
            btnToggleActive.Click += (_, __) => ToggleActive();
            btnToggleRole.Click += (_, __) => ToggleRole();
            btnDeleteUser.Click += (_, __) => DeleteUser();
            btnClose.Click += (_, __) => Close();
        }

        private void SetupGrid()
        {
            dataGridViewUsers.AutoGenerateColumns = false;
            dataGridViewUsers.ReadOnly = true;
            dataGridViewUsers.MultiSelect = false;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUsers.AllowUserToAddRows = false;
            dataGridViewUsers.AllowUserToDeleteRows = false;

            dataGridViewUsers.Columns.Clear();

            dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = "ID",
                Width = 70
            });

            dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Username",
                HeaderText = "Username",
                Width = 220
            });

            dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Role",
                HeaderText = "Role",
                Width = 90
            });

            dataGridViewUsers.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "IsActive",
                HeaderText = "Active",
                Width = 70
            });

            dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CreatedAt",
                HeaderText = "Created",
                Width = 180,
                DefaultCellStyle = { Format = "yyyy-MM-dd HH:mm" }
            });
        }

        private void Reload()
        {
            try
            {
                var list = _repo.GetUsers();
                _bind = new BindingList<UserRow>(list);
                dataGridViewUsers.DataSource = _bind;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load users: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private UserRow? Selected()
            => dataGridViewUsers.CurrentRow?.DataBoundItem as UserRow;

        private void AddUser()
        {
            var username = Prompt("Username (e.g. cashier1):");
            if (string.IsNullOrWhiteSpace(username)) return;

            var password = Prompt("Password:");
            if (string.IsNullOrWhiteSpace(password)) return;

            var role = Prompt("Role (ADMIN or USER):", "USER")?.Trim().ToUpperInvariant();
            if (role != "ADMIN" && role != "USER")
            {
                MessageBox.Show("Role must be ADMIN or USER.", "Invalid role",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _repo.CreateUser(username, password, role);
                Reload();
                MessageBox.Show("User created.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Create user failed: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetPassword()
        {
            var u = Selected();
            if (u == null)
            {
                MessageBox.Show("Select a user first.");
                return;
            }

            var newPass = Prompt($"New password for '{u.Username}':");
            if (string.IsNullOrWhiteSpace(newPass)) return;

            try
            {
                _repo.ResetPassword(u.Id, newPass);
                MessageBox.Show("Password reset.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reset failed: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleActive()
        {
            var u = Selected();
            if (u == null)
            {
                MessageBox.Show("Select a user first.");
                return;
            }

            if (u.Id == AppSession.UserId && u.IsActive)
            {
                MessageBox.Show("You cannot disable the currently signed-in user.",
                    "Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _repo.SetActive(u.Id, !u.IsActive);
                Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleRole()
        {
            var u = Selected();
            if (u == null)
            {
                MessageBox.Show("Select a user first.");
                return;
            }

            var newRole = u.Role == "ADMIN" ? "USER" : "ADMIN";

            if (u.Id == AppSession.UserId && u.Role == "ADMIN" && newRole == "USER")
            {
                MessageBox.Show("You cannot demote yourself while logged in.",
                    "Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _repo.SetRole(u.Id, newRole);
                Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Role change failed: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteUser()
        {
            var u = Selected();
            if (u == null)
            {
                MessageBox.Show("Select a user first.");
                return;
            }

            if (u.Id == AppSession.UserId)
            {
                MessageBox.Show("You cannot delete the currently signed-in user.",
                    "Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Delete user '{u.Username}'?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _repo.DeleteUser(u.Id);
                Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string? Prompt(string label, string defaultValue = "")
        {
            using var f = new Form
            {
                Width = 420,
                Height = 160,
                Text = "Input",
                StartPosition = FormStartPosition.CenterParent
            };

            var lbl = new Label { Left = 10, Top = 15, Width = 380, Text = label };
            var tb = new TextBox { Left = 10, Top = 40, Width = 380, Text = defaultValue };
            var ok = new Button { Text = "OK", Left = 230, Width = 75, Top = 75, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Cancel", Left = 315, Width = 75, Top = 75, DialogResult = DialogResult.Cancel };

            f.Controls.Add(lbl);
            f.Controls.Add(tb);
            f.Controls.Add(ok);
            f.Controls.Add(cancel);
            f.AcceptButton = ok;
            f.CancelButton = cancel;

            return f.ShowDialog() == DialogResult.OK ? tb.Text : null;
        }
    }
}
