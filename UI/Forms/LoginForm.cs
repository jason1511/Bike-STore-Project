using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class LoginForm : Form
    {
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
            var profile = AppServices.Profile;
            Text = $"{profile.StoreName} — Sign in";
            BackColor = UiTheme.Canvas;
            MinimumSize = new Size(620, 480);
            AutoScaleMode = AutoScaleMode.Dpi;
            tableRoot.BackColor = UiTheme.Canvas;
            panelLogin.Size = new Size(430, 365);
            panelLogin.Padding = new Padding(40);
            panelLogin.BackColor = Color.White;
            panelLogin.BorderStyle = BorderStyle.FixedSingle;

            var eyebrow = new Label { Text = "DESKTOP ADMIN", Tag = "accent", Left = 40, Top = 28, Width = 330, Height = 20, ForeColor = UiTheme.Accent, Font = new Font("Segoe UI Semibold", 8F) };
            var title = new Label { Text = profile.StoreName, Left = 40, Top = 51, Width = 345, Height = 34, ForeColor = UiTheme.Text, Font = new Font("Segoe UI Semibold", 15F) };
            var subtitle = new Label { Text = $"{profile.BackendLabel} · Sign in to manage catalogue, stock, invoices and service.", Tag = "muted", Left = 40, Top = 87, Width = 345, Height = 38, ForeColor = UiTheme.Muted, Font = new Font("Segoe UI", 9F) };
            label1.Location = new Point(40, 137); txtUser.Location = new Point(40, 161); txtUser.Width = 345; txtUser.Height = 31;
            label2.Location = new Point(40, 207); txtPass.Location = new Point(40, 231); txtPass.Width = 345; txtPass.Height = 31;
            btnLogin.Location = new Point(40, 293); btnLogin.Size = new Size(345, 42); btnLogin.BackColor = UiTheme.Sidebar; btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat; btnLogin.FlatAppearance.BorderSize = 0; btnLogin.Cursor = Cursors.Hand;
            panelLogin.Controls.Add(eyebrow); panelLogin.Controls.Add(title); panelLogin.Controls.Add(subtitle);
            if (profile.IsDemo)
            {
                var demo = new Label { Text = "Demo login: admin / admin123", Left = 40, Top = 268, Width = 345, Height = 20, ForeColor = UiTheme.Accent, Font = new Font("Segoe UI Semibold", 8.5F) };
                panelLogin.Controls.Add(demo); demo.BringToFront();
            }
            eyebrow.BringToFront(); title.BringToFront(); subtitle.BringToFront();
            UiTheme.Apply(this);
            btnLogin.BackColor = UiTheme.Sidebar; btnLogin.ForeColor = Color.White; btnLogin.FlatAppearance.BorderSize = 0;
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            var username = txtUser.Text?.Trim() ?? "";
            var password = txtPass.Text ?? "";

            btnLogin.Enabled = false;
            btnLogin.Text = "Signing in…";
            try
            {
                var result = await AppServices.Backend.LoginAsync(username, password);
                if (result.Success)
                {
                    AppSession.SignIn(result.UserId, result.Username, result.Role);

                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                MessageBox.Show(string.IsNullOrWhiteSpace(result.Error) ? "Login failed." : result.Error,
                    "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtPass.SelectAll(); txtPass.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnLogin.Enabled = true; btnLogin.Text = "Sign in";
            }
        }
    }
}
