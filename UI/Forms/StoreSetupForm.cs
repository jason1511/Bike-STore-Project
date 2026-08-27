using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public sealed class StoreSetupForm : Form
    {
        private readonly RadioButton _demo = new() { Text = "DEMO\nExplore with sample data" };
        private readonly RadioButton _local = new() { Text = "LOCAL SQLITE\nKeep data on this computer" };
        private readonly RadioButton _online = new() { Text = "ONLINE API\nConnect to a deployed store" };
        private readonly TextBox _profileName = Box();
        private readonly TextBox _storeName = Box();
        private readonly TextBox _shortName = Box(100);
        private readonly TextBox _databasePath = Box();
        private readonly TextBox _apiUrl = Box();
        private readonly TextBox _invoiceTitle = Box();
        private readonly NumericUpDown _lowStock = new() { Width = 100, Minimum = 0, Maximum = 1000, Value = 5 };
        private readonly ComboBox _culture = new() { Width = 170, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly Label _modeHelp = new() { AutoSize = true, Dock = DockStyle.Fill, Tag = "muted" };
        private readonly Label _connectionTitle = new() { AutoSize = true, Font = new Font("Segoe UI Semibold", 11F) };
        private readonly Label _securityNote = new() { AutoSize = true, Dock = DockStyle.Fill, Tag = "muted" };
        private readonly Button _save = new() { Text = "Save and continue", Width = 160, Height = 40, Tag = "primary" };
        private readonly Button _test = new() { Text = "Check connection", Width = 140, Height = 38 };
        private readonly Button _resetDemo = new() { Text = "Reset demo database", Width = 155, Height = 38 };
        private TableLayoutPanel _connectionFields = null!;
        private int _sqliteRow;
        private int _onlineRow;
        private readonly StoreProfile _initial;
        private bool _filling;

        public StoreProfile SelectedProfile { get; private set; }

        public StoreSetupForm(StoreProfile? current = null)
        {
            _initial = current ?? StoreProfile.CreateDemo();
            SelectedProfile = _initial;
            Text = current == null ? "Set up Bike Store Desktop" : "Store connection settings";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(940, 720);
            MinimumSize = new Size(760, 620);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = UiTheme.Canvas;
            _save.Text = current == null ? "Save and continue" : "Save changes";
            Build();
            _filling = true;
            Fill(_initial);
            _filling = false;
            UpdateMode();
            UiTheme.Apply(this);
            StylePrimaryButton(_save);
            StyleModeButtons();
        }

        private void Build()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 26, 30, 22),
                ColumnCount = 1,
                RowCount = 4,
                BackColor = UiTheme.Canvas
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var header = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Margin = new Padding(0, 0, 0, 18) };
            header.Controls.Add(new Label { Text = "STORE PROFILE", AutoSize = true, Tag = "accent", Font = new Font("Segoe UI Semibold", 8F) });
            header.Controls.Add(new Label { Text = "Set up your Bike Store workspace", AutoSize = true, Font = new Font("Segoe UI Semibold", 20F), Margin = new Padding(0, 3, 0, 0) });
            header.Controls.Add(new Label
            {
                Text = "Choose where this profile keeps its data. You can change the connection later from Store settings.",
                AutoSize = true,
                Dock = DockStyle.Fill,
                Tag = "muted",
                Margin = new Padding(0, 5, 0, 0)
            });
            root.Controls.Add(header, 0, 0);

            var modeSection = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Margin = new Padding(0, 0, 0, 18) };
            modeSection.Controls.Add(new Label { Text = "1. Choose a data source", AutoSize = true, Font = new Font("Segoe UI Semibold", 11F), Margin = new Padding(0, 0, 0, 8) });
            var modes = new TableLayoutPanel { Dock = DockStyle.Top, Height = 88, ColumnCount = 3, Margin = new Padding(0) };
            modes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            modes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            modes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334F));
            ConfigureModeButton(_demo, new Padding(0, 0, 7, 0));
            ConfigureModeButton(_local, new Padding(7, 0, 7, 0));
            ConfigureModeButton(_online, new Padding(7, 0, 0, 0));
            modes.Controls.Add(_demo, 0, 0); modes.Controls.Add(_local, 1, 0); modes.Controls.Add(_online, 2, 0);
            modeSection.Controls.Add(modes);
            var helpPanel = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, BackColor = Color.FromArgb(235, 244, 248), Padding = new Padding(13, 10, 13, 8), Margin = new Padding(0, 8, 0, 0) };
            helpPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            helpPanel.Controls.Add(_modeHelp, 0, 0);
            modeSection.Controls.Add(helpPanel);
            root.Controls.Add(modeSection, 0, 1);

            var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = new Padding(0), Padding = new Padding(0, 0, 8, 0) };
            var content = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Margin = new Padding(0) };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var identity = CreateCard();
            var identityLayout = CreateCardLayout();
            var identityTitle = new Label { Text = "2. Store details", AutoSize = true, Font = new Font("Segoe UI Semibold", 11F), Margin = new Padding(0, 0, 0, 5) };
            var identityHelp = new Label { Text = "These names appear in the app and on printed documents.", AutoSize = true, Dock = DockStyle.Fill, Tag = "muted", Margin = new Padding(0, 0, 0, 14) };
            identityLayout.Controls.Add(identityTitle, 0, 0);
            identityLayout.SetColumnSpan(identityTitle, 2);
            identityLayout.Controls.Add(identityHelp, 0, 1);
            identityLayout.SetColumnSpan(identityHelp, 2);
            Add(identityLayout, "Profile name", _profileName);
            Add(identityLayout, "Store name", _storeName);
            _shortName.Dock = DockStyle.Left;
            Add(identityLayout, "Short name", _shortName);
            _culture.Dock = DockStyle.Left;
            Add(identityLayout, "Language / currency", _culture);
            Add(identityLayout, "Invoice title", _invoiceTitle);
            _lowStock.Dock = DockStyle.Left;
            Add(identityLayout, "Low-stock warning", _lowStock);
            identity.Controls.Add(identityLayout);
            content.Controls.Add(identity);

            var connection = CreateCard();
            connection.Margin = new Padding(0, 14, 0, 0);
            _connectionFields = CreateCardLayout();
            _connectionFields.Controls.Add(_connectionTitle, 0, 0);
            _connectionFields.SetColumnSpan(_connectionTitle, 2);
            _connectionFields.Controls.Add(_securityNote, 0, 1);
            _connectionFields.SetColumnSpan(_securityNote, 2);

            var databaseEditor = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, Tag = "card", Margin = new Padding(0) };
            databaseEditor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            databaseEditor.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _databasePath.Dock = DockStyle.Fill;
            var browse = new Button { Text = "Browse…", Width = 90, Height = 30, Margin = new Padding(8, 0, 0, 0) };
            browse.Click += (_, __) => BrowseDatabase();
            databaseEditor.Controls.Add(_databasePath, 0, 0); databaseEditor.Controls.Add(browse, 1, 0);
            _sqliteRow = Add(_connectionFields, "SQLite file", databaseEditor);
            _onlineRow = Add(_connectionFields, "Store API URL", _apiUrl);
            connection.Controls.Add(_connectionFields);
            content.Controls.Add(connection);
            scroll.Controls.Add(content);
            root.Controls.Add(scroll, 0, 2);

            var footer = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 18, 0, 0) };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var secondaryActions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false, Margin = new Padding(0) };
            secondaryActions.Controls.Add(_resetDemo); secondaryActions.Controls.Add(_test);
            var mainActions = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Margin = new Padding(0) };
            var cancel = new Button { Text = "Cancel", Width = 92, Height = 40, DialogResult = DialogResult.Cancel, Margin = new Padding(0, 0, 8, 0) };
            mainActions.Controls.Add(cancel); mainActions.Controls.Add(_save);
            footer.Controls.Add(secondaryActions, 0, 0); footer.Controls.Add(mainActions, 1, 0);
            root.Controls.Add(footer, 0, 3);

            _save.Click += (_, __) => SaveProfile();
            _test.Click += async (_, __) => await TestAsync();
            _resetDemo.Click += (_, __) => ResetDemo();
            Controls.Add(root);
            CancelButton = cancel;

            _culture.Items.Add(new CultureChoice("Bahasa Indonesia / IDR", "id-ID", "IDR"));
            _culture.Items.Add(new CultureChoice("English (Australia) / AUD", "en-AU", "AUD"));
            _culture.SelectedIndex = 0;
            _shortName.MaxLength = 5;
            _demo.CheckedChanged += (_, __) => ModeChanged(StoreBackendMode.Demo, _demo.Checked);
            _local.CheckedChanged += (_, __) => ModeChanged(StoreBackendMode.Local, _local.Checked);
            _online.CheckedChanged += (_, __) => ModeChanged(StoreBackendMode.Cloudflare, _online.Checked);
        }

        private void Fill(StoreProfile profile)
        {
            _profileName.Text = profile.ProfileName; _storeName.Text = profile.StoreName; _shortName.Text = profile.ShortName;
            _invoiceTitle.Text = profile.InvoiceTitle; _lowStock.Value = Math.Min(_lowStock.Maximum, Math.Max(_lowStock.Minimum, profile.LowStockThreshold));
            _databasePath.Text = string.IsNullOrWhiteSpace(profile.DatabasePath)
                ? (profile.IsDemo ? AppPaths.DemoDatabasePath : AppPaths.LocalDatabasePath) : profile.DatabasePath;
            _apiUrl.Text = profile.ApiBaseUrl;
            SelectCulture(profile.Culture);
            if (profile.Backend == StoreBackendMode.Cloudflare) _online.Checked = true;
            else if (profile.Backend == StoreBackendMode.Local) _local.Checked = true;
            else _demo.Checked = true;
        }

        private void ModeChanged(StoreBackendMode mode, bool isChecked)
        {
            if (!isChecked) return;
            if (!_filling)
            {
                var defaults = mode switch
                {
                    StoreBackendMode.Local => StoreProfile.CreateLocal(),
                    StoreBackendMode.Cloudflare => StoreProfile.CreateCvNiaga(),
                    _ => StoreProfile.CreateDemo()
                };
                _profileName.Text = defaults.ProfileName; _storeName.Text = defaults.StoreName; _shortName.Text = defaults.ShortName;
                _databasePath.Text = defaults.DatabasePath; _apiUrl.Text = defaults.ApiBaseUrl;
                _invoiceTitle.Text = defaults.InvoiceTitle; _lowStock.Value = defaults.LowStockThreshold;
                SelectCulture(defaults.Culture);
            }
            UpdateMode();
        }

        private void UpdateMode()
        {
            if (_local.Checked && (string.IsNullOrWhiteSpace(_databasePath.Text) || PathsEqual(_databasePath.Text, AppPaths.DemoDatabasePath)))
                _databasePath.Text = AppPaths.LocalDatabasePath;
            if (_demo.Checked) _databasePath.Text = AppPaths.DemoDatabasePath;
            if (_online.Checked && string.IsNullOrWhiteSpace(_apiUrl.Text)) _apiUrl.Text = StoreProfile.CreateCvNiaga().ApiBaseUrl;
            SetRowVisible(_connectionFields, _sqliteRow, _local.Checked);
            SetRowVisible(_connectionFields, _onlineRow, _online.Checked);
            _test.Visible = _online.Checked;
            _resetDemo.Visible = _demo.Checked && File.Exists(AppPaths.DemoDatabasePath);
            _connectionTitle.Text = _demo.Checked ? "3. Demo storage" : _local.Checked ? "3. Local database" : "3. Online connection";
            _securityNote.Text = _demo.Checked
                ? "No connection details are required. Demo data is stored separately and can be reset at any time."
                : _local.Checked
                    ? "Choose one SQLite file for this profile. The app verifies that the folder and database can be opened before saving."
                    : "Only the HTTPS server address is saved here. Cloudflare/D1 credentials remain on the server; users sign in normally.";
            _modeHelp.Text = _demo.Checked
                ? "Best for exploring: safe sample data, stored locally, with no production connection."
                : _local.Checked
                    ? "Best for one computer or an offline store: persistent data in a SQLite file you control."
                    : "Best for a deployed store: the desktop app uses an authenticated HTTPS API backed by Cloudflare Workers and D1.";
            StyleModeButtons();
        }

        private StoreProfile BuildProfile()
        {
            var culture = _culture.SelectedItem as CultureChoice ?? new CultureChoice("Bahasa Indonesia / IDR", "id-ID", "IDR");
            var mode = _online.Checked ? StoreBackendMode.Cloudflare : _local.Checked ? StoreBackendMode.Local : StoreBackendMode.Demo;
            return new StoreProfile
            {
                Backend = mode,
                ProfileName = Required(_profileName.Text, "Profile name"),
                StoreName = Required(_storeName.Text, "Store name"),
                ShortName = Required(_shortName.Text, "Short name").ToUpperInvariant(),
                DatabasePath = mode == StoreBackendMode.Demo ? AppPaths.DemoDatabasePath : _databasePath.Text.Trim(),
                ApiBaseUrl = _apiUrl.Text.Trim().TrimEnd('/'),
                Culture = culture.Culture,
                CurrencyCode = culture.Currency,
                InvoiceTitle = Required(_invoiceTitle.Text, "Invoice title"),
                LowStockThreshold = (int)_lowStock.Value
            };
        }

        private void SaveProfile()
        {
            try
            {
                var profile = BuildProfile();
                if (profile.Backend == StoreBackendMode.Local && string.IsNullOrWhiteSpace(profile.DatabasePath))
                    throw new InvalidOperationException("Choose a SQLite database file.");
                if (profile.Backend == StoreBackendMode.Local)
                    Database.ValidateDatabaseFile(profile.DatabasePath);
                if (profile.IsOnline && (!Uri.TryCreate(profile.ApiBaseUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps))
                    throw new InvalidOperationException("Enter a valid HTTPS online store URL.");
                StoreConfiguration.Save(profile); SelectedProfile = profile; DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Store setup", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private async Task TestAsync()
        {
            try
            {
                _test.Enabled = false; _test.Text = "Testing…";
                var profile = BuildProfile();
                using var backend = new CloudflareStoreBackend(profile);
                await backend.TestConnectionAsync();
                MessageBox.Show("The online store API is reachable.", "Connection successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Connection failed", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            finally { _test.Enabled = true; _test.Text = "Check connection"; }
        }

        private void BrowseDatabase()
        {
            try
            {
                using var dialog = new SaveFileDialog { Filter = "SQLite database (*.db)|*.db|All files (*.*)|*.*", FileName = Path.GetFileName(_databasePath.Text), InitialDirectory = Path.GetDirectoryName(_databasePath.Text) };
                if (dialog.ShowDialog(this) == DialogResult.OK) _databasePath.Text = dialog.FileName;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Choose database file", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void ResetDemo()
        {
            if (MessageBox.Show("Reset the demo database to its original sample data?", "Reset demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                Database.DeleteDatabaseFile(AppPaths.DemoDatabasePath);
                _resetDemo.Visible = false;
                MessageBox.Show("Demo data will be recreated when you continue.", "Demo reset");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Demo reset failed", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private static string Required(string value, string label) => string.IsNullOrWhiteSpace(value) ? throw new InvalidOperationException(label + " is required.") : value.Trim();
        private void SelectCulture(string culture)
        {
            for (var i = 0; i < _culture.Items.Count; i++)
                if (_culture.Items[i] is CultureChoice choice && choice.Culture == culture) { _culture.SelectedIndex = i; return; }
        }
        private static bool PathsEqual(string left, string right)
        {
            try { return Path.GetFullPath(left).Equals(Path.GetFullPath(right), StringComparison.OrdinalIgnoreCase); }
            catch { return false; }
        }
        private static TextBox Box(int width = 360) => new() { Width = width, Dock = DockStyle.Fill };
        private static int Add(TableLayoutPanel table, string label, Control control)
        {
            var row = table.RowCount++; table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Controls.Add(new Label { Text = label, AutoSize = true, Padding = new Padding(0, 7, 0, 0), Margin = new Padding(0, 4, 12, 8) }, 0, row);
            control.Margin = new Padding(0, 4, 0, 8);
            table.Controls.Add(control, 1, row);
            return row;
        }
        private static Panel CreateCard()
        {
            return new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = UiTheme.Card,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(22),
                Tag = "card",
                Margin = new Padding(0)
            };
        }
        private static TableLayoutPanel CreateCardLayout()
        {
            var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 2, Tag = "card", Margin = new Padding(0) };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 175));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            return table;
        }
        private static void ConfigureModeButton(RadioButton button, Padding margin)
        {
            button.Appearance = Appearance.Button;
            button.AutoSize = false;
            button.Dock = DockStyle.Fill;
            button.Margin = margin;
            button.Padding = new Padding(18, 12, 12, 10);
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Font = new Font("Segoe UI Semibold", 9.5F);
            button.FlatStyle = FlatStyle.Flat;
            button.Cursor = Cursors.Hand;
        }
        private void StyleModeButtons()
        {
            foreach (var button in new[] { _demo, _local, _online })
            {
                var selected = button.Checked;
                button.BackColor = selected ? Color.FromArgb(229, 241, 247) : UiTheme.Card;
                button.ForeColor = selected ? UiTheme.Accent : UiTheme.Text;
                button.FlatAppearance.BorderColor = selected ? UiTheme.Accent : UiTheme.Border;
                button.FlatAppearance.BorderSize = selected ? 2 : 1;
                button.FlatAppearance.CheckedBackColor = Color.FromArgb(229, 241, 247);
            }
        }
        private static void StylePrimaryButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = UiTheme.Sidebar;
            button.ForeColor = Color.White;
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(10, 0, 10, 0);
        }
        private static void SetRowVisible(TableLayoutPanel table, int row, bool visible)
        {
            foreach (Control control in table.Controls)
                if (table.GetRow(control) == row) control.Visible = visible;
            table.RowStyles[row].SizeType = visible ? SizeType.AutoSize : SizeType.Absolute;
            table.RowStyles[row].Height = 0;
        }
        private sealed record CultureChoice(string Label, string Culture, string Currency) { public override string ToString() => Label; }
    }
}
