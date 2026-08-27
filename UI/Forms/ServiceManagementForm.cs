using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
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
        private readonly ComboBox _filter = new() { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly DataGridView _history = Grid();
        private DataRowView? _printRow;

        public ServiceManagementForm()
        {
            Text = $"Bike Store - Service - {AppSession.Username} ({AppSession.Role})";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(700, 500);
            AutoScaleMode = AutoScaleMode.Dpi;
            foreach (var value in StatusChoices(false)) _status.Items.Add(value);
            _status.SelectedIndex = 0;
            foreach (var value in StatusChoices(true)) _filter.Items.Add(value);
            _filter.SelectedIndex = 0; _filter.SelectedIndexChanged += (_, __) => LoadHistory();
            _brand.CharacterCasing = CharacterCasing.Upper;
            _type.CharacterCasing = CharacterCasing.Upper;
            _color.CharacterCasing = CharacterCasing.Upper;

            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildEntry());
            tabs.TabPages.Add(BuildHistory());
            Controls.Add(tabs);
            Load += (_, __) => LoadHistory();
            _search.TextChanged += (_, __) => LoadHistory();
        }

        private TabPage BuildEntry()
        {
            var tab = new TabPage("New service");
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
            AddField(root, "Colour", _color);
            AddField(root, "Service type *", _serviceType);
            AddField(root, "Status", _status);
            AddField(root, "Service cost", _cost);
            AddField(root, "Notes", _notes);
            var actions = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft, WrapContents = true };
            var savePrint = new Button { Text = "Save and print", Width = 150, Height = 40 };
            savePrint.Click += (_, __) => Save(print: true);
            var save = new Button { Text = "Save service", Width = 130, Height = 40 };
            save.Click += (_, __) => Save(print: false);
            actions.Controls.Add(savePrint); actions.Controls.Add(save); root.Controls.Add(actions);
            tab.Controls.Add(root); return tab;
        }

        private TabPage BuildHistory()
        {
            var tab = new TabPage("Service history");
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, Padding = new Padding(12) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoScroll = true, WrapContents = true };
            actions.Controls.Add(new Label { Text = "Search", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
            actions.Controls.Add(_search);
            actions.Controls.Add(new Label { Text = "Status", AutoSize = true, Padding = new Padding(8, 7, 0, 0) });
            actions.Controls.Add(_filter);
            var refresh = new Button { Text = "Refresh" }; refresh.Click += (_, __) => LoadHistory();
            var print = new Button { Text = "Print service job" }; print.Click += (_, __) => PrintSelected();
            var status = new Button { Text = "Update status", Enabled = AppSession.IsAdmin }; status.Click += (_, __) => UpdateStatus();
            var edit = new Button { Text = "Edit details", Enabled = AppSession.IsAdmin }; edit.Click += (_, __) => EditSelected();
            var delete = new Button { Text = "Delete service job", Enabled = AppSession.IsAdmin, AutoSize = true }; delete.Click += (_, __) => DeleteSelected();
            actions.Controls.Add(refresh); actions.Controls.Add(print); actions.Controls.Add(edit); actions.Controls.Add(status); actions.Controls.Add(delete);
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
                    cmd.Parameters.AddWithValue("$status", SelectedStatus(_status));
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
                    $"{number}; customer={_customer.Text.Trim()}; status={SelectedStatus(_status)}");
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
WHERE ($status='ALL' OR UPPER(COALESCE(service_status,'RECEIVED'))=$status)
AND ($q='' OR UPPER(COALESCE(service_number,'')) LIKE $like OR UPPER(COALESCE(customer_name,'')) LIKE $like
 OR UPPER(brand) LIKE $like OR UPPER(type) LIKE $like)
ORDER BY datetime(COALESCE(created_at,date_time)) DESC,id DESC;";
            var q = _search.Text.Trim().ToUpperInvariant(); cmd.Parameters.AddWithValue("$q", q); cmd.Parameters.AddWithValue("$like", $"%{q}%");
            cmd.Parameters.AddWithValue("$status", SelectedStatus(_filter));
            using var reader = cmd.ExecuteReader(); var table = new DataTable(); table.Load(reader); _history.DataSource = table;
            if (_history.Columns.Contains("id")) _history.Columns["id"].Visible = false;
            if (_history.Columns.Contains("service_cost"))
            {
                _history.Columns["service_cost"].DefaultCellStyle.Format = "C0";
                _history.Columns["service_cost"].DefaultCellStyle.FormatProvider = StoreFormat.Culture;
            }
        }

        private void UpdateStatus()
        {
            var id = SelectedId(); if (id == 0) return;
            var next = DialogPrompt.Show(this, "Update service status", "Enter Received, In progress, Completed, or Cancelled:", "Completed")
                .Trim().Replace(' ', '_').ToUpperInvariant();
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

        private void EditSelected()
        {
            if (_history.CurrentRow?.DataBoundItem is not DataRowView row) { MessageBox.Show("Select a service first."); return; }
            using var dialog = new ServiceEditDialog(row);
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            var id = Convert.ToInt32(row["id"]); using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                using var cmd = conn.CreateCommand(); cmd.Transaction = tx;
                cmd.CommandText = @"
UPDATE services SET customer_name=$customer,customer_phone=$phone,brand=$brand,type=$type,color=$color,
 service_type=$service,service_cost=$cost,notes=$notes WHERE id=$id;";
                cmd.Parameters.AddWithValue("$customer", dialog.Customer); cmd.Parameters.AddWithValue("$phone", Db(dialog.Phone));
                cmd.Parameters.AddWithValue("$brand", dialog.Brand); cmd.Parameters.AddWithValue("$type", dialog.BikeType);
                cmd.Parameters.AddWithValue("$color", Db(dialog.Color)); cmd.Parameters.AddWithValue("$service", dialog.ServiceType);
                cmd.Parameters.AddWithValue("$cost", (double)dialog.Cost); cmd.Parameters.AddWithValue("$notes", Db(dialog.Notes)); cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery(); LocalAdminRepository.WriteAudit(conn, tx, "UPDATE_SERVICE", "services", id, $"customer={dialog.Customer}"); tx.Commit(); LoadHistory();
            }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show(ex.Message, "Service update failed"); }
        }

        private void DeleteSelected()
        {
            var id = SelectedId(); if (id == 0) return;
            var reason = DialogPrompt.Show(this, "Delete service record", "Reason for deletion:", "Administrative correction");
            if (string.IsNullOrWhiteSpace(reason) || MessageBox.Show("Permanently delete this service record?", "Delete service", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                using var cmd = conn.CreateCommand(); cmd.Transaction = tx; cmd.CommandText = "DELETE FROM services WHERE id=$id;"; cmd.Parameters.AddWithValue("$id", id);
                cmd.ExecuteNonQuery(); LocalAdminRepository.WriteAudit(conn, tx, "DELETE_SERVICE", "services", id, $"reason={reason}"); tx.Commit(); _printRow = null; LoadHistory();
            }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show(ex.Message, "Delete failed"); }
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
                g.DrawString(AppServices.Profile.StoreName.ToUpperInvariant(), title, Brushes.Black, x, y); y += 40;
                g.DrawString("FORM SERVICE SEPEDA LISTRIK", heading, Brushes.Black, x, y); y += 30;
                foreach (var line in new[]
                {
                    $"Nomor: {_printRow["service_number"]}", $"Tanggal: {_printRow["created_at"]}",
                    $"Pelanggan: {_printRow["customer_name"]}", $"Telepon: {_printRow["customer_phone"]}",
                    $"Sepeda: {_printRow["brand"]} {_printRow["type"]} {_printRow["color"]}",
                    $"Jenis service: {_printRow["service_type"]}", $"Status: {_printRow["service_status"]}",
                    $"Biaya: {StoreFormat.Money(Convert.ToDecimal(_printRow["service_cost"]))}", $"Catatan: {_printRow["notes"]}"
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
        private static IEnumerable<StatusChoice> StatusChoices(bool includeAll)
        {
            if (includeAll) yield return new StatusChoice("ALL", "All statuses");
            yield return new StatusChoice("RECEIVED", "Received");
            yield return new StatusChoice("IN_PROGRESS", "In progress");
            yield return new StatusChoice("COMPLETED", "Completed");
            yield return new StatusChoice("CANCELLED", "Cancelled");
        }
        private static string SelectedStatus(ComboBox combo) => (combo.SelectedItem as StatusChoice)?.Code ?? "ALL";
        private sealed record StatusChoice(string Code, string Label) { public override string ToString() => Label; }
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

    internal sealed class ServiceEditDialog : Form
    {
        private readonly TextBox _customer = Box(); private readonly TextBox _phone = Box(); private readonly TextBox _brand = Box();
        private readonly TextBox _type = Box(); private readonly TextBox _color = Box(); private readonly TextBox _service = Box();
        private readonly TextBox _notes = new() { Width = 260, Multiline = true, Height = 60 };
        private readonly NumericUpDown _cost = new() { Width = 160, Maximum = 1_000_000_000, ThousandsSeparator = true };
        public string Customer => _customer.Text.Trim(); public string Phone => _phone.Text.Trim(); public string Brand => _brand.Text.Trim().ToUpperInvariant();
        public string BikeType => _type.Text.Trim().ToUpperInvariant(); public string Color => _color.Text.Trim().ToUpperInvariant();
        public string ServiceType => _service.Text.Trim(); public string Notes => _notes.Text.Trim(); public decimal Cost => _cost.Value;

        public ServiceEditDialog(DataRowView row)
        {
            Text = "Edit service details"; StartPosition = FormStartPosition.CenterParent; ClientSize = new Size(520, 500);
            MinimumSize = new Size(470, 420); FormBorderStyle = FormBorderStyle.Sizable; MinimizeBox = false; AutoScaleMode = AutoScaleMode.Dpi;
            _customer.Text = Convert.ToString(row["customer_name"]); _phone.Text = Convert.ToString(row["customer_phone"]);
            _brand.Text = Convert.ToString(row["brand"]); _type.Text = Convert.ToString(row["type"]); _color.Text = Convert.ToString(row["color"]);
            _service.Text = Convert.ToString(row["service_type"]); _notes.Text = Convert.ToString(row["notes"]); _cost.Value = Math.Min(_cost.Maximum, Convert.ToDecimal(row["service_cost"]));
            var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(20), AutoScroll = true };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145)); table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Add(table, "Customer *", _customer); Add(table, "Phone", _phone); Add(table, "Brand *", _brand); Add(table, "Type/model *", _type);
            Add(table, "Colour", _color); Add(table, "Service type *", _service); Add(table, "Cost", _cost); Add(table, "Notes", _notes);
            var actions = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
            var save = new Button { Text = "Save changes", Width = 120, DialogResult = DialogResult.OK }; var cancel = new Button { Text = "Cancel", Width = 90, DialogResult = DialogResult.Cancel };
            save.Click += (_, __) => { if (string.IsNullOrWhiteSpace(Customer) || string.IsNullOrWhiteSpace(Brand) || string.IsNullOrWhiteSpace(BikeType) || string.IsNullOrWhiteSpace(ServiceType)) { MessageBox.Show("Customer, brand, model and service type are required."); DialogResult = DialogResult.None; } };
            actions.Controls.Add(save); actions.Controls.Add(cancel); Add(table, "", actions); Controls.Add(table); AcceptButton = save; CancelButton = cancel; UiTheme.Apply(this);
        }
        private static TextBox Box() => new() { Width = 260 };
        private static void Add(TableLayoutPanel table, string label, Control control)
        { var row = table.RowCount++; table.RowStyles.Add(new RowStyle(SizeType.AutoSize)); table.Controls.Add(new Label { Text = label, AutoSize = true, Padding = new Padding(0, 7, 0, 0) }, 0, row); table.Controls.Add(control, 1, row); }
    }
}
