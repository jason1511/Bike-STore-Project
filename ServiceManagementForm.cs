using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public sealed class ServiceManagementForm : Form
    {
        private readonly TextBox _customer = Box(180);
        private readonly TextBox _phone = Box(140);
        private readonly TextBox _address = Box(220);
        private readonly TextBox _brand = Box(130);
        private readonly TextBox _type = Box(150);
        private readonly TextBox _color = Box(110);
        private readonly TextBox _serviceType = Box(180);
        private readonly TextBox _notes = new() { Width = 230, Multiline = true, Height = 48 };
        private readonly TextBox _search = Box(220);
        private readonly NumericUpDown _cost = new() { Minimum = 0, Maximum = 1_000_000_000, ThousandsSeparator = true, Width = 140 };
        private readonly ComboBox _status = new() { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly DataGridView _history = Grid();
        private DataRowView? _printRow;

        public ServiceManagementForm()
        {
            Text = $"Bike Store - Service - {AppSession.Username} ({AppSession.Role})";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1050, 650);
            foreach (var value in new[] { "RECEIVED", "IN_PROGRESS", "COMPLETED", "CANCELLED" }) _status.Items.Add(value);
            _status.SelectedIndex = 0;
            _brand.CharacterCasing = CharacterCasing.Upper;
            _type.CharacterCasing = CharacterCasing.Upper;
            _color.CharacterCasing = CharacterCasing.Upper;

            var menu = new MainMenuControl { Dock = DockStyle.Top };
            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildEntry());
            tabs.TabPages.Add(BuildHistory());
            Controls.Add(tabs);
            Controls.Add(menu);
            Load += (_, __) => LoadHistory();
            _search.TextChanged += (_, __) => LoadHistory();
        }

        private TabPage BuildEntry()
        {
            var tab = new TabPage("New Service");
            var root = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, AutoScroll = true, WrapContents = true,
                Padding = new Padding(20), FlowDirection = FlowDirection.LeftToRight
            };
            AddField(root, "Customer *", _customer);
            AddField(root, "Phone", _phone);
            AddField(root, "Address", _address);
            AddField(root, "Brand *", _brand);
            AddField(root, "Type / model *", _type);
            AddField(root, "Color", _color);
            AddField(root, "Service type *", _serviceType);
            AddField(root, "Status", _status);
            AddField(root, "Service cost", _cost);
            AddField(root, "Notes", _notes);
            var actions = new FlowLayoutPanel { Width = 920, Height = 60, FlowDirection = FlowDirection.RightToLeft };
            var savePrint = new Button { Text = "Save & print", Width = 150, Height = 40 };
            savePrint.Click += (_, __) => Save(print: true);
            var save = new Button { Text = "Save service", Width = 130, Height = 40 };
            save.Click += (_, __) => Save(print: false);
            actions.Controls.Add(savePrint); actions.Controls.Add(save); root.Controls.Add(actions);
            tab.Controls.Add(root); return tab;
        }

        private TabPage BuildHistory()
        {
            var tab = new TabPage("Service History");
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, Padding = new Padding(12) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            actions.Controls.Add(new Label { Text = "Search", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
            actions.Controls.Add(_search);
            var refresh = new Button { Text = "Refresh" }; refresh.Click += (_, __) => LoadHistory();
            var print = new Button { Text = "Print selected" }; print.Click += (_, __) => PrintSelected();
            var status = new Button { Text = "Update status", Enabled = AppSession.IsAdmin }; status.Click += (_, __) => UpdateStatus();
            actions.Controls.Add(refresh); actions.Controls.Add(print); actions.Controls.Add(status);
            root.Controls.Add(actions, 0, 0); root.Controls.Add(_history, 0, 1); tab.Controls.Add(root); return tab;
        }

        private void Save(bool print)
        {
            if (string.IsNullOrWhiteSpace(_customer.Text) || string.IsNullOrWhiteSpace(_brand.Text) ||
                string.IsNullOrWhiteSpace(_type.Text) || string.IsNullOrWhiteSpace(_serviceType.Text))
            {
                MessageBox.Show("Customer, brand, model, and service type are required."); return;
            }
            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();
            try
            {
                long id;
                var now = DateTime.Now;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
INSERT INTO services
(brand,type,color,quantity,service_cost,notes,date_time,service_number,customer_name,customer_phone,
 customer_address,service_type,service_status,created_by_user_id,created_by_username,created_at)
VALUES ($brand,$type,$color,1,$cost,$notes,$at,NULL,$customer,$phone,$address,$service,$status,$uid,$user,$at);
SELECT last_insert_rowid();";
                    cmd.Parameters.AddWithValue("$brand", _brand.Text.Trim().ToUpperInvariant());
                    cmd.Parameters.AddWithValue("$type", _type.Text.Trim().ToUpperInvariant());
                    cmd.Parameters.AddWithValue("$color", Db(_color.Text));
                    cmd.Parameters.AddWithValue("$cost", (double)_cost.Value);
                    cmd.Parameters.AddWithValue("$notes", Db(_notes.Text));
                    cmd.Parameters.AddWithValue("$at", now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("$customer", _customer.Text.Trim());
                    cmd.Parameters.AddWithValue("$phone", Db(_phone.Text));
                    cmd.Parameters.AddWithValue("$address", Db(_address.Text));
                    cmd.Parameters.AddWithValue("$service", _serviceType.Text.Trim());
                    cmd.Parameters.AddWithValue("$status", _status.Text);
                    cmd.Parameters.AddWithValue("$uid", AppSession.UserId);
                    cmd.Parameters.AddWithValue("$user", AppSession.Username);
                    id = Convert.ToInt64(cmd.ExecuteScalar() ?? 0L);
                }
                var number = $"SRV-{now:yyyyMMdd}-{id:000}";
                using (var update = conn.CreateCommand())
                {
                    update.Transaction = tx;
                    update.CommandText = "UPDATE services SET service_number=$number WHERE id=$id;";
                    update.Parameters.AddWithValue("$number", number); update.Parameters.AddWithValue("$id", id); update.ExecuteNonQuery();
                }
                LocalAdminRepository.WriteAudit(conn, tx, "CREATE_SERVICE", "services", id,
                    $"{number}; customer={_customer.Text.Trim()}; status={_status.Text}");
                tx.Commit();
                MessageBox.Show($"Service {number} saved.");
                LoadHistory();
                if (print) PrintService((int)id);
                ClearEntry();
            }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show(ex.Message, "Service not saved"); }
        }

        private void LoadHistory()
        {
            using var conn = Database.OpenConnection(); using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT id,COALESCE(service_number,'LEGACY-'||id) AS service_number,
 COALESCE(customer_name,'') AS customer_name,COALESCE(customer_phone,'') AS customer_phone,
 brand,type,COALESCE(color,'') AS color,COALESCE(service_type,'GENERAL') AS service_type,
 COALESCE(service_status,'RECEIVED') AS service_status,service_cost,COALESCE(notes,'') AS notes,
 COALESCE(created_by_username,'') AS created_by,COALESCE(created_at,date_time) AS created_at
FROM services
WHERE $q='' OR UPPER(COALESCE(service_number,'')) LIKE $like OR UPPER(COALESCE(customer_name,'')) LIKE $like
 OR UPPER(brand) LIKE $like OR UPPER(type) LIKE $like
ORDER BY datetime(COALESCE(created_at,date_time)) DESC,id DESC;";
            var q = _search.Text.Trim().ToUpperInvariant(); cmd.Parameters.AddWithValue("$q", q); cmd.Parameters.AddWithValue("$like", $"%{q}%");
            using var reader = cmd.ExecuteReader(); var table = new DataTable(); table.Load(reader); _history.DataSource = table;
            if (_history.Columns.Contains("id")) _history.Columns["id"].Visible = false;
            if (_history.Columns.Contains("service_cost"))
            {
                _history.Columns["service_cost"].DefaultCellStyle.Format = "C0";
                _history.Columns["service_cost"].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
        }

        private void UpdateStatus()
        {
            var id = SelectedId(); if (id == 0) return;
            var next = DialogPrompt.Show(this, "Update service status", "Enter RECEIVED, IN_PROGRESS, COMPLETED, or CANCELLED:", "COMPLETED").ToUpperInvariant();
            if (Array.IndexOf(new[] { "RECEIVED", "IN_PROGRESS", "COMPLETED", "CANCELLED" }, next) < 0)
            { MessageBox.Show("Invalid service status."); return; }
            using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                using var cmd = conn.CreateCommand(); cmd.Transaction = tx;
                cmd.CommandText = "UPDATE services SET service_status=$status,completed_at=CASE WHEN $status='COMPLETED' THEN $at ELSE completed_at END WHERE id=$id;";
                cmd.Parameters.AddWithValue("$status", next); cmd.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("o")); cmd.Parameters.AddWithValue("$id", id); cmd.ExecuteNonQuery();
                LocalAdminRepository.WriteAudit(conn, tx, "UPDATE_SERVICE_STATUS", "services", id, $"status={next}"); tx.Commit(); LoadHistory();
            }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show(ex.Message); }
        }

        private void PrintSelected() { var id = SelectedId(); if (id != 0) PrintService(id); }
        private int SelectedId()
        {
            if (_history.CurrentRow?.DataBoundItem is not DataRowView row) { MessageBox.Show("Select a service first."); return 0; }
            _printRow = row; return Convert.ToInt32(row["id"]);
        }

        private void PrintService(int id)
        {
            if (_printRow == null || Convert.ToInt32(_printRow["id"]) != id)
            {
                LoadHistory();
                foreach (DataGridViewRow row in _history.Rows)
                    if (row.DataBoundItem is DataRowView view && Convert.ToInt32(view["id"]) == id) { _printRow = view; break; }
            }
            if (_printRow == null) return;
            var doc = new PrintDocument { DocumentName = Convert.ToString(_printRow["service_number"]) ?? "Service" };
            doc.PrintPage += (_, e) =>
            {
                if (e.Graphics == null || _printRow == null) return;
                var g = e.Graphics; var x = e.MarginBounds.Left; var y = e.MarginBounds.Top;
                using var title = new Font("Segoe UI", 18, FontStyle.Bold); using var heading = new Font("Segoe UI", 11, FontStyle.Bold); using var body = new Font("Segoe UI", 10);
                g.DrawString("CV NIAGA BERSAMA ABADI", title, Brushes.Black, x, y); y += 40;
                g.DrawString("FORM SERVICE SEPEDA LISTRIK", heading, Brushes.Black, x, y); y += 30;
                foreach (var line in new[]
                {
                    $"Nomor: {_printRow["service_number"]}", $"Tanggal: {_printRow["created_at"]}",
                    $"Pelanggan: {_printRow["customer_name"]}", $"Telepon: {_printRow["customer_phone"]}",
                    $"Sepeda: {_printRow["brand"]} {_printRow["type"]} {_printRow["color"]}",
                    $"Jenis service: {_printRow["service_type"]}", $"Status: {_printRow["service_status"]}",
                    $"Biaya: Rp {Convert.ToDecimal(_printRow["service_cost"]):N0}", $"Catatan: {_printRow["notes"]}"
                }) { g.DrawString(line, body, Brushes.Black, x, y); y += 24; }
                y += 50; g.DrawString("Teknisi / Staff", body, Brushes.Black, x, y); g.DrawString("Pelanggan", body, Brushes.Black, e.MarginBounds.Right - 160, y);
            };
            using var preview = new PrintPreviewDialog { Document = doc, Width = 1000, Height = 750 }; preview.ShowDialog(this);
        }

        private void ClearEntry()
        {
            foreach (var box in new[] { _customer, _phone, _address, _brand, _type, _color, _serviceType, _notes }) box.Clear();
            _cost.Value = 0; _status.SelectedIndex = 0; _printRow = null;
        }

        private static object Db(string value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
        private static TextBox Box(int width) => new() { Width = width };
        private static void AddField(FlowLayoutPanel panel, string label, Control control)
        {
            var field = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Margin = new Padding(8) };
            field.Controls.Add(new Label { Text = label, AutoSize = true }); field.Controls.Add(control); panel.Controls.Add(field);
        }
        private static DataGridView Grid() => new()
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false
        };
    }
}
