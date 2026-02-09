using System;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class MainMenuControl : UserControl
    {
        public MainMenuControl()
        {
            InitializeComponent();

            // Operations
            menuInventory.Click += (_, __) => SwitchTo(() => new InventoryForm());
            menuSales.Click += (_, __) => SwitchTo(() => new SalesForm());
            menuService.Click += (_, __) => SwitchTo(() => new ServiceForm());

            // Logs
            menuTransactionLog.Click += (_, __) => SwitchTo(() => new TransactionLogForm());
            menuServiceLog.Click += (_, __) => SwitchTo(() => new ServiceLogForm());

            // Admin
            menuUserManagement.Click += (_, __) => OpenUserManagement();

            // File
            menuLogout.Click += (_, __) => Logout();
            menuExit.Click += (_, __) => Application.Exit();

            // Update menu visibility + title when this control is shown/returns focus
            HandleCreated += (_, __) => ApplySessionUi();
            VisibleChanged += (_, __) => { if (Visible) ApplySessionUi(); };
        }

        private void ApplySessionUi()
        {
            ApplyRoleVisibility();
            UpdateParentTitle();
        }

        private void ApplyRoleVisibility()
        {
            adminToolStripMenuItem.Visible = AppSession.IsAdmin;
            menuUserManagement.Visible = AppSession.IsAdmin;
        }

        private void UpdateParentTitle()
        {
            var form = FindForm();
            if (form == null) return;

            // Extract clean form name (remove previous user suffix if any)
            var title = form.Text;

            // Remove anything after the second dash
            // "Bike Store - Inventory - admin (ADMIN)" → "Inventory"
            if (title.StartsWith("Bike Store - ", StringComparison.OrdinalIgnoreCase))
            {
                title = title.Substring("Bike Store - ".Length);
                var dash = title.IndexOf(" - ");
                if (dash >= 0)
                    title = title.Substring(0, dash);
            }

            var formName = title.Trim();

            if (!AppSession.IsSignedIn)
            {
                form.Text = $"Bike Store - {formName}";
                return;
            }

            form.Text = $"Bike Store - {formName} - {AppSession.Username} ({AppSession.Role})";
        }

        private void SwitchTo(Func<Form> createForm)
        {
            var current = FindForm();
            if (current == null) return;

            var next = createForm();
            next.StartPosition = FormStartPosition.CenterScreen;

            current.Hide();

            next.FormClosed += (_, __) =>
            {
                if (!current.IsDisposed)
                {
                    current.Show();
                    ApplySessionUi(); // ensure title/menu correct when coming back
                }
            };

            next.Show();
        }

        private void Logout()
        {
            var confirm = MessageBox.Show(
                "Logout from the application?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            var current = FindForm();
            if (current == null) return;

            AppSession.SignOut();
            ApplySessionUi(); // update title/menu immediately

            current.Hide();

            using var login = new LoginForm();
            if (login.ShowDialog() == DialogResult.OK)
            {
                // logged in again (could be different user/role)
                ApplySessionUi();
                current.Show();
            }
            else
            {
                Application.Exit();
            }
        }

        private void OpenUserManagement()
        {
            var current = FindForm();
            if (current == null) return;

            if (!AppSession.IsAdmin)
            {
                MessageBox.Show("Admin access required.", "Access denied",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var f = new UserManagementForm
            {
                StartPosition = FormStartPosition.CenterParent
            };

            f.ShowDialog(current);

            // after closing admin form, refresh title/menu (role might have changed)
            ApplySessionUi();
        }
    }
}
