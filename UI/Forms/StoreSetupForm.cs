using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public sealed class StoreSetupForm : Form
    {
        private readonly RadioButton _demo = new() { Text = "Try the demo", AutoSize = true };
        private readonly RadioButton _local = new() { Text = "Use a local database", AutoSize = true };
        private readonly RadioButton _online = new() { Text = "Connect to an online store", AutoSize = true };
        private readonly TextBox _profileName = Box();
        private readonly TextBox _storeName = Box();
        private readonly TextBox _shortName = Box(100);
        private readonly TextBox _databasePath = Box();
        private readonly TextBox _apiUrl = Box();
        private readonly TextBox _invoiceTitle = Box();
        private readonly NumericUpDown _lowStock = new() { Width = 100, Minimum = 0, Maximum = 1000, Value = 5 };
        private readonly ComboBox _culture = new() { Width = 170, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly Label _modeHelp = new() { AutoSize = true, MaximumSize = new Size(550, 0), ForeColor = UiTheme.Muted };
        private readonly Button _save = new() { Text = "Save and continue", Width = 145, Height = 36 };
        private readonly Button _test = new() { Text = "Test connection", Width = 125, Height = 36 };
        private readonly Button _resetDemo = new() { Text = "Reset demo data", Width = 125, Height = 36 };
        private readonly StoreProfile _initial;
        private bool _filling;

        public StoreProfile SelectedProfile { get; private set; }

        public StoreSetupForm(StoreProfile? current = null)
        {
            _initial = current ?? StoreProfile.CreateDemo();
            SelectedProfile = _initial;
            Text = current == null ? "Set up Bike Store Desktop" : "Store connection settings";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(720, 650);
            MinimumSize = new Size(680, 610);
            AutoScaleMode = AutoScaleMode.Dpi;
            Build();
            _filling = true;
            Fill(_initial);
            _filling = false;
            UpdateMode();
            UiTheme.Apply(this);
        }

        private void Build()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(34), ColumnCount = 1, AutoScroll = true };
            root.Controls.Add(new Label { Text = "Choose how this app stores data", AutoSize = true, Font = new Font("Segoe UI Semibold", 18F), ForeColor = UiTheme.Text });
            root.Controls.Add(new Label { Text = "Use safe sample data, keep a database on this computer, or connect to a store's Cloudflare API.", AutoSize = true, MaximumSize = new Size(610, 0), ForeColor = UiTheme.Muted, Margin = new Padding(0, 4, 0, 18) });

            var modes = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false, Margin = new Padding(0, 0, 0, 8) };
            modes.Controls.Add(_demo); modes.Controls.Add(_local); modes.Controls.Add(_online); root.Controls.Add(modes);
            root.Controls.Add(_modeHelp);

            var fields = new TableLayoutPanel { AutoSize = true, ColumnCount = 2, Margin = new Padding(0, 18, 0, 0) };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Add(fields, "Profile name", _profileName);
            Add(fields, "Store name", _storeName);
            Add(fields, "Short name", _shortName);
            Add(fields, "Language / currency", _culture);
            Add(fields, "Invoice title", _invoiceTitle);
            Add(fields, "Low-stock level", _lowStock);
            var dbPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
            var browse = new Button { Text = "Browse…", Width = 86, Height = 28 };
            browse.Click += (_, __) => BrowseDatabase(); dbPanel.Controls.Add(_databasePath); dbPanel.Controls.Add(browse);
            Add(fields, "SQLite file", dbPanel);
            Add(fields, "Online store URL", _apiUrl);
            root.Controls.Add(fields);

            var note = new Label
            {
                Text = "The desktop app only stores the server address. Cloudflare/D1 credentials are never placed in this file; online access uses the normal store login.",
                AutoSize = true, MaximumSize = new Size(610, 0), ForeColor = UiTheme.Muted, Margin = new Padding(0, 18, 0, 12)
            };
            root.Controls.Add(note);

            var actions = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Top };
            var cancel = new Button { Text = "Cancel", Width = 90, Height = 36, DialogResult = DialogResult.Cancel };
            _save.Click += (_, __) => SaveProfile(); _test.Click += async (_, __) => await TestAsync(); _resetDemo.Click += (_, __) => ResetDemo();
            actions.Controls.Add(_save); actions.Controls.Add(cancel); actions.Controls.Add(_test); actions.Controls.Add(_resetDemo); root.Controls.Add(actions);
            Controls.Add(root); CancelButton = cancel;

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
            _databasePath.Enabled = _local.Checked;
            _apiUrl.Enabled = _online.Checked;
            _test.Visible = _online.Checked;
            _resetDemo.Visible = _demo.Checked && File.Exists(AppPaths.DemoDatabasePath);
            _modeHelp.Text = _demo.Checked
                ? "A resettable, local SQLite playground. It never connects to real company data."
                : _local.Checked
                    ? "A persistent SQLite file for a single computer or offline store."
                    : "Uses the store's HTTPS API. For CV Niaga Bersama this API is backed by Cloudflare Workers and D1.";
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
            finally { _test.Enabled = true; _test.Text = "Test connection"; }
        }

        private void BrowseDatabase()
        {
            using var dialog = new SaveFileDialog { Filter = "SQLite database (*.db)|*.db|All files (*.*)|*.*", FileName = Path.GetFileName(_databasePath.Text), InitialDirectory = Path.GetDirectoryName(_databasePath.Text) };
            if (dialog.ShowDialog(this) == DialogResult.OK) _databasePath.Text = dialog.FileName;
        }

        private void ResetDemo()
        {
            if (MessageBox.Show("Reset the demo database to its original sample data?", "Reset demo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            Database.DeleteDatabaseFile(AppPaths.DemoDatabasePath);
            _resetDemo.Visible = false;
            MessageBox.Show("Demo data will be recreated when you continue.", "Demo reset");
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
        private static TextBox Box(int width = 360) => new() { Width = width };
        private static void Add(TableLayoutPanel table, string label, Control control)
        {
            var row = table.RowCount++; table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Controls.Add(new Label { Text = label, AutoSize = true, Padding = new Padding(0, 7, 0, 0) }, 0, row); table.Controls.Add(control, 1, row);
        }
        private sealed record CultureChoice(string Label, string Culture, string Currency) { public override string ToString() => Label; }
    }
}
