using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public sealed class AdminDashboardForm : Form
    {
        private readonly Panel _sidebar = new() { Dock = DockStyle.Left, Width = 230, BackColor = UiTheme.Sidebar };
        private readonly Panel _host = new() { Dock = DockStyle.Fill, BackColor = UiTheme.Canvas, Padding = new Padding(0) };
        private readonly Label _pageTitle = new() { AutoSize = true, Font = new Font("Segoe UI Semibold", 17F), ForeColor = UiTheme.Text };
        private readonly Label _pageSubtitle = new() { AutoSize = true, Font = new Font("Segoe UI", 9F), ForeColor = UiTheme.Muted };
        private readonly Label _userLabel = new() { AutoSize = true, Font = new Font("Segoe UI Semibold", 9.5F), ForeColor = UiTheme.Text };
        private readonly Label _roleLabel = new() { AutoSize = true, Font = new Font("Segoe UI", 8F), ForeColor = UiTheme.Muted };
        private readonly FlowLayoutPanel _nav = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(12, 12, 12, 12) };
        private readonly Dictionary<string, Button> _navButtons = new(StringComparer.OrdinalIgnoreCase);
        private readonly ToolTip _tooltips = new();
        private Control? _currentPage;
        private string _currentKey = "";

        public AdminDashboardForm()
        {
            Text = $"{AppServices.Profile.StoreName} — Desktop Admin";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = UiTheme.Canvas;

            var brand = BuildBrandHeader();
            var footer = BuildSidebarFooter();
            _sidebar.Controls.Add(_nav);
            _sidebar.Controls.Add(footer);
            _sidebar.Controls.Add(brand);

            var content = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Canvas };
            var header = BuildTopHeader();
            content.Controls.Add(_host);
            content.Controls.Add(header);

            Controls.Add(content);
            Controls.Add(_sidebar);
            RebuildNavigation();
            UpdateSessionLabels();
            Shown += (_, __) => ShowPage("dashboard");
            FormClosed += (_, __) => _currentPage?.Dispose();
        }

        private Panel BuildBrandHeader()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 96, BackColor = UiTheme.Sidebar, Padding = new Padding(16, 18, 10, 10) };
            var mark = new Label
            {
                Text = AppServices.Profile.ShortName, Width = 48, Height = 48, TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White, ForeColor = UiTheme.Sidebar, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Dock = DockStyle.Left
            };
            var displayName = AppServices.Profile.StoreName.Length > 22 ? AppServices.Profile.ProfileName : AppServices.Profile.StoreName;
            var title = new Label { Text = $"{displayName.ToUpperInvariant()}\n{AppServices.Profile.BackendLabel}", ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 9F), Dock = DockStyle.Fill, Padding = new Padding(12, 3, 0, 0) };
            panel.Controls.Add(title); panel.Controls.Add(mark); return panel;
        }

        private Panel BuildSidebarFooter()
        {
            var panel = new Panel { Dock = DockStyle.Bottom, Height = 152, BackColor = UiTheme.Sidebar, Padding = new Padding(14, 10, 14, 14) };
            var line = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(70, 95, 94) };
            var user = new Label { Name = "sidebarUser", Dock = DockStyle.Top, Height = 28, ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 9.5F), Padding = new Padding(0, 8, 0, 0) };
            var role = new Label { Name = "sidebarRole", Dock = DockStyle.Top, Height = 25, ForeColor = Color.FromArgb(180, 199, 196), Font = new Font("Segoe UI", 8F) };
            var logout = new Button
            {
                Text = "Sign out", Dock = DockStyle.Bottom, Height = 34, FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.SidebarHover, ForeColor = Color.White, Cursor = Cursors.Hand, Tag = "nav"
            };
            var settings = new Button
            {
                Text = "Store settings", Dock = DockStyle.Bottom, Height = 32, FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.Sidebar, ForeColor = Color.FromArgb(210, 223, 220), Cursor = Cursors.Hand, Tag = "nav"
            };
            settings.FlatAppearance.BorderColor = Color.FromArgb(90, 115, 112); settings.Click += (_, __) => OpenStoreSettings();
            logout.FlatAppearance.BorderColor = Color.FromArgb(90, 115, 112); logout.Click += (_, __) => Logout();
            panel.Controls.Add(logout); panel.Controls.Add(settings); panel.Controls.Add(role); panel.Controls.Add(user); panel.Controls.Add(line); return panel;
        }

        private Panel BuildTopHeader()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 76, BackColor = Color.White, Padding = new Padding(24, 12, 22, 10) };
            panel.Paint += (_, e) => e.Graphics.DrawLine(new Pen(UiTheme.Border), 0, panel.Height - 1, panel.Width, panel.Height - 1);
            var text = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            _pageTitle.Location = new Point(0, 0); _pageSubtitle.Location = new Point(2, 38); text.Controls.Add(_pageTitle); text.Controls.Add(_pageSubtitle);
            var account = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 215, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(12, 4, 0, 0), BackColor = Color.White };
            account.Controls.Add(_userLabel); account.Controls.Add(_roleLabel);
            panel.Controls.Add(text); panel.Controls.Add(account); return panel;
        }

        private void RebuildNavigation()
        {
            _nav.Controls.Clear(); _navButtons.Clear();
            AddNav("dashboard", "Overview", "Daily operations and key figures");
            AddSection("OPERATIONS");
            AddNav("inventory", "Bicycles & stock", "Website-style bicycle, colour and stock management");
            if (!AppServices.Backend.SupportsFullDesktopWorkflow) return;
            AddNav("sales", "Sales & invoices", "Create, print and review invoices");
            AddNav("service", "Service", "Service jobs and status");
            if (AppSession.IsAdmin)
            {
                AddSection("ADMINISTRATION");
                AddNav("brands", "Brands", "Brand records and visibility");
                AddNav("stock", "Stock movements", "Incoming and outgoing stock");
                AddNav("reports", "Reports", "Sales, service and profit");
                AddNav("users", "Users", "Accounts and roles");
                AddNav("activity", "Activity", "Audit trail");
            }
        }

        private void AddSection(string text)
        {
            _nav.Controls.Add(new Label { Text = text, Width = 194, Height = 30, ForeColor = Color.FromArgb(142, 166, 162), Font = new Font("Segoe UI Semibold", 7.5F), Padding = new Padding(8, 12, 0, 0), Margin = new Padding(0, 5, 0, 2) });
        }

        private void AddNav(string key, string text, string tooltip)
        {
            var button = new Button
            {
                Name = "nav_" + key, Text = text, Tag = "nav", Width = 194, Height = 44,
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(14, 0, 0, 0), Margin = new Padding(0, 1, 0, 1),
                FlatStyle = FlatStyle.Flat, BackColor = UiTheme.Sidebar, ForeColor = Color.FromArgb(229, 237, 235), Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0; button.FlatAppearance.MouseOverBackColor = UiTheme.SidebarHover;
            button.Click += (_, __) => ShowPage(key); _tooltips.SetToolTip(button, tooltip);
            _navButtons[key] = button; _nav.Controls.Add(button);
        }

        public void ShowPage(string key)
        {
            if (_currentKey.Equals(key, StringComparison.OrdinalIgnoreCase) && key != "dashboard") return;
            Control page;
            string title, subtitle;
            switch (key.ToLowerInvariant())
            {
                case "inventory": page = Embed(new InventoryForm()); title = "Bicycles & stock"; subtitle = "Manage website-style bike models, colour variants and local stock."; break;
                case "sales": page = Embed(new InvoiceManagementForm()); title = "Sales & invoices"; subtitle = "Create multi-item invoices, record payments and print customer documents."; break;
                case "service": page = Embed(new ServiceManagementForm()); title = "Service"; subtitle = "Create service jobs, follow progress and print service documents."; break;
                case "brands": page = Embed(new LocalAdminCenterForm(LocalAdminSection.Brands)); title = "Brands"; subtitle = "Maintain the brand directory used by the catalogue."; break;
                case "stock": page = Embed(new LocalAdminCenterForm(LocalAdminSection.StockMovements)); title = "Stock movements"; subtitle = "Review every receipt, sale, adjustment and void restoration."; break;
                case "reports": page = Embed(new LocalAdminCenterForm(LocalAdminSection.Reports)); title = "Reports"; subtitle = "Analyse sales, service income, gross profit and stock movement."; break;
                case "users": page = Embed(new UserManagementForm()); title = "Users"; subtitle = "Manage staff access, roles, status and password resets."; break;
                case "activity": page = Embed(new LocalAdminCenterForm(LocalAdminSection.Activity)); title = "Activity"; subtitle = "Trace important actions back to the signed-in user."; break;
                default:
                    if (AppServices.Backend.SupportsFullDesktopWorkflow)
                    {
                        var dashboard = new DashboardPageControl(); dashboard.NavigateRequested += ShowPage; page = dashboard;
                    }
                    else page = new OnlineOverviewControl();
                    title = "Overview"; subtitle = $"{AppServices.Profile.StoreName} · {AppServices.Profile.BackendLabel}"; key = "dashboard"; break;
            }

            _currentPage?.Dispose(); _host.Controls.Clear(); _currentPage = page; page.Dock = DockStyle.Fill; _host.Controls.Add(page);
            _pageTitle.Text = title; _pageSubtitle.Text = subtitle; _currentKey = key; SetActiveNav(key);
            UiTheme.Apply(page);
        }

        private static Control Embed(Form form)
        {
            form.WindowState = FormWindowState.Normal; form.TopLevel = false; form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill; form.AutoScaleMode = AutoScaleMode.Dpi;
            HideLegacyChrome(form);
            form.Show(); return form;
        }

        private static void HideLegacyChrome(Control root)
        {
            foreach (Control control in root.Controls.Cast<Control>().ToArray())
            {
                if (control is MenuStrip ||
                    (control is Button button &&
                     (button.Text.Equals("Close", StringComparison.OrdinalIgnoreCase) ||
                      button.Text.Equals("Logout", StringComparison.OrdinalIgnoreCase))))
                { control.Visible = false; continue; }
                HideLegacyChrome(control);
            }
        }

        private void SetActiveNav(string key)
        {
            foreach (var pair in _navButtons)
            {
                var active = pair.Key.Equals(key, StringComparison.OrdinalIgnoreCase);
                pair.Value.BackColor = active ? Color.FromArgb(64, 98, 94) : UiTheme.Sidebar;
                pair.Value.ForeColor = Color.White;
                pair.Value.Font = new Font("Segoe UI", 9.5F, active ? FontStyle.Bold : FontStyle.Regular);
            }
        }

        private void UpdateSessionLabels()
        {
            _userLabel.Text = AppSession.Username;
            _roleLabel.Text = AppSession.IsAdmin ? "Administrator" : "Staff";
            var user = FindControl(_sidebar, "sidebarUser") as Label; if (user != null) user.Text = AppSession.Username;
            var role = FindControl(_sidebar, "sidebarRole") as Label; if (role != null) role.Text = AppSession.IsAdmin ? "ADMINISTRATOR" : "STAFF";
        }

        private void Logout()
        {
            if (MessageBox.Show("Sign out of the desktop admin?", "Sign out", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            AppSession.SignOut(); Hide();
            AppServices.Backend.SignOut();
            using var login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK) { Close(); return; }
            RebuildNavigation(); UpdateSessionLabels(); _currentKey = ""; Show(); ShowPage("dashboard");
        }

        private void OpenStoreSettings()
        {
            using var setup = new StoreSetupForm(AppServices.Profile);
            if (setup.ShowDialog(this) != DialogResult.OK) return;
            MessageBox.Show("The app will restart to apply the new store profile.", "Store settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Restart();
            Close();
        }

        private static Control? FindControl(Control root, string name)
        {
            if (root.Name == name) return root;
            foreach (Control child in root.Controls) { var found = FindControl(child, name); if (found != null) return found; }
            return null;
        }
    }
}
