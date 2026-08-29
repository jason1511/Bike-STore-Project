using System;
using System.ComponentModel;
using System.Linq;
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
            ApplyLocalizedText();

            // ✅ Admin-only guard
            if (!AppSession.IsAdmin)
            {
                MessageBox.Show(Strings.Get("User_AccessDenied"),
                    Strings.Get("Common_PermissionDenied"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
            dataGridViewUsers.SelectionChanged += (_, __) => UpdateSelectionActions();
            tableRoot.SizeChanged += (_, __) => UpdateResponsiveLayout();
            Shown += (_, __) => UpdateResponsiveLayout();
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
                HeaderText = Strings.Get("Grid_id"),
                Width = 70
            });

            dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Username",
                HeaderText = Strings.Get("Grid_username"),
                Width = 220
            });

            dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "role",
                DataPropertyName = "Role",
                HeaderText = Strings.Get("Grid_role"),
                Width = 90
            });

            dataGridViewUsers.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "is_active",
                DataPropertyName = "IsActive",
                HeaderText = Strings.Get("Grid_is_active"),
                Width = 70
            });

            dataGridViewUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CreatedAt",
                HeaderText = Strings.Get("Grid_created_at"),
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
                UpdateSelectionActions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Strings.Format("User_LoadFailed", ex.Message),
                    Strings.Get("Common_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private UserRow? Selected()
            => dataGridViewUsers.CurrentRow?.DataBoundItem as UserRow;

        private void AddUser()
        {
            var username = Prompt(Strings.Get("User_UsernamePrompt"));
            if (string.IsNullOrWhiteSpace(username)) return;

            var password = Prompt(Strings.Get("User_PasswordPrompt"), password: true);
            if (string.IsNullOrWhiteSpace(password)) return;

            var roleInput = Prompt(Strings.Get("User_RolePrompt"), Strings.Get("Role_USER"))?.Trim();
            var role = roleInput?.ToUpperInvariant() switch
            {
                "ADMIN" or "ADMINISTRATOR" or "ADMINISTRATOR/ADMIN" => "ADMIN",
                "USER" or "STAFF" or "STAF" => "USER",
                _ => ""
            };
            if (role.Length == 0)
            {
                MessageBox.Show(Strings.Get("User_InvalidRoleMessage"), Strings.Get("User_InvalidRoleTitle"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _repo.CreateUser(username, password, role);
                Reload();
                MessageBox.Show(Strings.Get("User_Created"), Strings.Get("Common_Success"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Strings.Format("User_CreateFailed", ex.Message),
                    Strings.Get("Common_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetPassword()
        {
            var u = Selected();
            if (u == null)
            {
                MessageBox.Show(Strings.Get("User_SelectFirst"));
                return;
            }

            var newPass = Prompt(Strings.Format("User_NewPasswordPrompt", u.Username), password: true);
            if (string.IsNullOrWhiteSpace(newPass)) return;

            try
            {
                _repo.ResetPassword(u.Id, newPass);
                MessageBox.Show(Strings.Get("User_PasswordReset"), Strings.Get("Common_Success"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Strings.Format("User_ResetFailed", ex.Message),
                    Strings.Get("Common_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleActive()
        {
            var u = Selected();
            if (u == null)
            {
                MessageBox.Show(Strings.Get("User_SelectFirst"));
                return;
            }

            if (u.Id == AppSession.UserId && u.IsActive)
            {
                MessageBox.Show(Strings.Get("User_CannotDisableSelf"),
                    Strings.Get("Common_Blocked"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _repo.SetActive(u.Id, !u.IsActive);
                Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Strings.Format("User_UpdateFailed", ex.Message),
                    Strings.Get("Common_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleRole()
        {
            var u = Selected();
            if (u == null)
            {
                MessageBox.Show(Strings.Get("User_SelectFirst"));
                return;
            }

            var newRole = u.Role == "ADMIN" ? "USER" : "ADMIN";

            if (u.Id == AppSession.UserId && u.Role == "ADMIN" && newRole == "USER")
            {
                MessageBox.Show(Strings.Get("User_CannotDemoteSelf"),
                    Strings.Get("Common_Blocked"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _repo.SetRole(u.Id, newRole);
                Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Strings.Format("User_RoleFailed", ex.Message),
                    Strings.Get("Common_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteUser()
        {
            var u = Selected();
            if (u == null)
            {
                MessageBox.Show(Strings.Get("User_SelectFirst"));
                return;
            }

            if (u.Id == AppSession.UserId)
            {
                MessageBox.Show(Strings.Get("User_CannotDeleteSelf"),
                    Strings.Get("Common_Blocked"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                Strings.Format("User_DeleteQuestion", u.Username),
                Strings.Get("User_ConfirmDelete"),
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
                MessageBox.Show(Strings.Format("User_DeleteFailed", ex.Message),
                    Strings.Get("Common_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSelectionActions()
        {
            var user = Selected();
            var hasSelection = user != null;
            btnResetPassword.Enabled = hasSelection;
            btnToggleActive.Enabled = hasSelection;
            btnToggleRole.Enabled = hasSelection;
            btnDeleteUser.Enabled = hasSelection;
            btnToggleActive.Text = Strings.Get(user?.IsActive == false ? "User_Enable" : "User_Disable");
            btnToggleRole.Text = Strings.Get(user?.Role == "ADMIN" ? "User_MakeStaff" : "User_MakeAdmin");
            UiTheme.StyleButton(btnToggleActive, user?.IsActive == true);
        }

        private void UpdateResponsiveLayout()
        {
            if (tableRoot.ClientSize.Width <= 0 || tableRoot.ClientSize.Height <= 0) return;
            var stacked = tableRoot.ClientSize.Width < 760;
            tableRoot.SuspendLayout();
            panelButtons.SuspendLayout();
            dataGridViewUsers.Dock = DockStyle.None;
            panelButtons.Dock = DockStyle.None;
            foreach (Control button in panelButtons.Controls) button.Height = 36;
            if (stacked)
            {
                panelButtons.FlowDirection = FlowDirection.LeftToRight;
                panelButtons.WrapContents = true;
                panelButtons.AutoSize = false;
                foreach (Control button in panelButtons.Controls) button.Width = 155;
                var visibleButtons = panelButtons.Controls.Cast<Control>().Count(x => x.Visible);
                var perRow = Math.Max(1, (tableRoot.ClientSize.Width - panelButtons.Padding.Horizontal) / 161);
                var rows = Math.Max(1, (int)Math.Ceiling(visibleButtons / (double)perRow));
                var actionHeight = panelButtons.Padding.Vertical + rows * 42 + 4;
                dataGridViewUsers.SetBounds(0, 0, tableRoot.ClientSize.Width, Math.Max(0, tableRoot.ClientSize.Height - actionHeight));
                panelButtons.SetBounds(0, dataGridViewUsers.Bottom, tableRoot.ClientSize.Width, actionHeight);
            }
            else
            {
                panelButtons.FlowDirection = FlowDirection.TopDown;
                panelButtons.WrapContents = false;
                panelButtons.AutoSize = false;
                foreach (Control button in panelButtons.Controls) button.Width = 190;
                const int actionWidth = 220;
                dataGridViewUsers.SetBounds(0, 0, Math.Max(0, tableRoot.ClientSize.Width - actionWidth), tableRoot.ClientSize.Height);
                panelButtons.SetBounds(dataGridViewUsers.Right, 0, actionWidth, tableRoot.ClientSize.Height);
            }
            panelButtons.ResumeLayout();
            tableRoot.ResumeLayout();
        }

        private void ApplyLocalizedText()
        {
            Text = Strings.Get("User_Title");
            btnAddUser.Text = Strings.Get("User_Add");
            btnResetPassword.Text = Strings.Get("User_ResetPassword");
            btnToggleActive.Text = Strings.Get("User_Disable");
            btnToggleRole.Text = Strings.Get("User_MakeAdmin");
            btnDeleteUser.Text = Strings.Get("User_Delete");
            btnClose.Text = Strings.Get("Common_Close");
        }

        private static string? Prompt(string label, string defaultValue = "", bool password = false)
        {
            using var f = new Form
            {
                Width = 420,
                Height = 160,
                Text = Strings.Get("User_Details"),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                AutoScaleMode = AutoScaleMode.Dpi
            };

            var lbl = new Label { Left = 10, Top = 15, Width = 380, Text = label };
            var tb = new TextBox { Left = 10, Top = 40, Width = 380, Text = defaultValue };
            tb.UseSystemPasswordChar = password;
            var ok = new Button { Text = Strings.Get("Common_OK"), Left = 230, Width = 75, Top = 75, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = Strings.Get("Common_Cancel"), Left = 315, Width = 75, Top = 75, DialogResult = DialogResult.Cancel };

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
