using System;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class LoginForm : Form
    {
        private readonly UserRepository _users = new();

        public LoginForm()
        {
            InitializeComponent();

            // UX: Enter to login
            AcceptButton = btnLogin;

            // Focus username on open
            Shown += (_, __) => txtUser.Focus();

            // Hook click (in case you didn't wire it in the designer)
            btnLogin.Click += BtnLogin_Click;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            var username = txtUser.Text?.Trim() ?? "";
            var password = txtPass.Text ?? "";

            if (_users.TryLogin(username, password, out var userId, out var role, out var error))
            {
                // Save session
                AppSession.SignIn(userId, username.Trim().ToLowerInvariant(), role);

                // Close dialog with OK so Program.cs can continue
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            MessageBox.Show(
                string.IsNullOrWhiteSpace(error) ? "Login failed." : error,
                "Login",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtPass.SelectAll();
            txtPass.Focus();
        }
    }
}
