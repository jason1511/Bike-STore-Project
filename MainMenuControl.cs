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

            ApplyRoleVisibility();
        }

        private void ApplyRoleVisibility()
        {
            var isAdmin = AppSession.IsAdmin;

            adminToolStripMenuItem.Visible = isAdmin;
            menuUserManagement.Visible = isAdmin;
        }

        private void SwitchTo(Func<Form> createForm)
        {
            var current = FindForm();
            if (current == null) return;

            var next = createForm();
            next.StartPosition = FormStartPosition.CenterScreen;

            next.FormClosed += (_, __) => current.Close();

            current.Hide();
            next.Show();
        }
        private void OpenUserManagement()
        {
            var current = FindForm();
            if (current == null) return;

            // extra safety (even though menu is hidden for non-admin)
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

            // modal: closing it returns you to the same screen
            f.ShowDialog(current);
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

            // Clear session
            AppSession.SignOut();

            var current = FindForm();
            if (current == null) return;

            // Show login again
            using var login = new LoginForm();
            if (login.ShowDialog() == DialogResult.OK)
            {
                // User logged in again → reopen main screen
                var next = new InventoryForm();
                next.StartPosition = FormStartPosition.CenterScreen;

                current.Hide();
                next.Show();
                current.Close();
            }
            else
            {
                // User cancelled login → exit app
                Application.Exit();
            }
        }
    }
}
