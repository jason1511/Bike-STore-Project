using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class LoginForm : Form
    {
        private readonly UserRepository _users = new();

        public LoginForm()
        {
            InitializeComponent();
            ApplyLoginDesign();

            // UX: Enter to login
            AcceptButton = btnLogin;

            // Focus username on open
            Shown += (_, __) => txtUser.Focus();

            // Hook click (in case you didn't wire it in the designer)
            btnLogin.Click += BtnLogin_Click;
        }

        private void ApplyLoginDesign()
        {
            Text = "CV Niaga Bersama Abadi — Sign in";
            BackColor = UiTheme.Canvas;
            MinimumSize = new Size(900, 620);
            tableRoot.BackColor = UiTheme.Canvas;
            panelLogin.Size = new Size(430, 365);
            panelLogin.Padding = new Padding(40);
            panelLogin.BackColor = Color.White;
            panelLogin.BorderStyle = BorderStyle.FixedSingle;

            var eyebrow = new Label { Text = "DESKTOP ADMIN", Tag = "accent", Left = 40, Top = 28, Width = 330, Height = 20, ForeColor = UiTheme.Accent, Font = new Font("Segoe UI Semibold", 8F) };
            var title = new Label { Text = "CV Niaga Bersama Abadi", Left = 40, Top = 51, Width = 345, Height = 34, ForeColor = UiTheme.Text, Font = new Font("Segoe UI Semibold", 15F) };
            var subtitle = new Label { Text = "Sign in to manage catalogue, stock, invoices and service.", Tag = "muted", Left = 40, Top = 87, Width = 345, Height = 38, ForeColor = UiTheme.Muted, Font = new Font("Segoe UI", 9F) };
            label1.Location = new Point(40, 137); txtUser.Location = new Point(40, 161); txtUser.Width = 345; txtUser.Height = 31;
            label2.Location = new Point(40, 207); txtPass.Location = new Point(40, 231); txtPass.Width = 345; txtPass.Height = 31;
            btnLogin.Location = new Point(40, 293); btnLogin.Size = new Size(345, 42); btnLogin.BackColor = UiTheme.Sidebar; btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat; btnLogin.FlatAppearance.BorderSize = 0; btnLogin.Cursor = Cursors.Hand;
            panelLogin.Controls.Add(eyebrow); panelLogin.Controls.Add(title); panelLogin.Controls.Add(subtitle);
            eyebrow.BringToFront(); title.BringToFront(); subtitle.BringToFront();
            UiTheme.Apply(this);
            btnLogin.BackColor = UiTheme.Sidebar; btnLogin.ForeColor = Color.White; btnLogin.FlatAppearance.BorderSize = 0;
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
