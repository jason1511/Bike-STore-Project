using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public enum LocalAdminSection
    {
        Brands,
        StockMovements,
        Reports,
        Activity
    }

    public sealed class LocalAdminCenterForm : Form
    {
        private readonly DataGridView _brands = Grid();
        private readonly DataGridView _movements = Grid();
        private readonly DataGridView _activity = Grid();
        private readonly DataGridView _report = Grid();
        private readonly DataGridView _breakdown = Grid();
        private readonly DateTimePicker _from = new() { Width = 130 };
        private readonly DateTimePicker _to = new() { Width = 130 };
        private readonly Label _summary = new() { AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
        private string _reportSummary = "";

        public LocalAdminCenterForm(LocalAdminSection? singleSection = null)
        {
            if (!AppSession.IsAdmin)
                throw new InvalidOperationException("Admin access required.");

            Text = $"Bike Store - Local Admin - {AppSession.Username} (ADMIN)";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1000, 620);
            WindowState = FormWindowState.Normal;
            MinimumSize = new Size(700, 500);
            AutoScaleMode = AutoScaleMode.Dpi;
            _from.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _to.Value = DateTime.Today;

            if (singleSection == null)
            {
                var tabs = new TabControl { Dock = DockStyle.Fill };
                tabs.TabPages.Add(BuildBrands());
                tabs.TabPages.Add(BuildMovements());
                tabs.TabPages.Add(BuildReports());
                tabs.TabPages.Add(BuildActivity());
                Controls.Add(tabs);
            }
            else
            {
                var page = singleSection.Value switch
                {
                    LocalAdminSection.Brands => BuildBrands(),
                    LocalAdminSection.StockMovements => BuildMovements(),
                    LocalAdminSection.Reports => BuildReports(),
                    _ => BuildActivity()
                };
                if (page.Controls.Count > 0)
                {
                    var content = page.Controls[0];
                    page.Controls.Remove(content);
                    content.Dock = DockStyle.Fill;
                    Controls.Add(content);
                }
            }
            Load += (_, __) => { LoadBrands(); LoadMovements(); LoadActivity(); GenerateReport(); };
        }

        private TabPage BuildBrands()
        {
            var tab = new TabPage("Brands");
            var root = RootWithActions(out var actions);
            var add = new Button { Text = "Add brand" };
            add.Click += (_, __) => AddBrand();
            var rename = new Button { Text = "Rename selected" };
            rename.Click += (_, __) => RenameBrand();
            var toggle = new Button { Text = "Deactivate brand", AutoSize = true };
            toggle.Click += (_, __) => ToggleBrand();
            void UpdateToggleText()
            {
                var active = _brands.CurrentRow?.DataBoundItem is DataRowView row && Convert.ToInt32(row["is_active"]) == 1;
                toggle.Text = active ? "Deactivate brand" : "Activate brand";
                UiTheme.StyleButton(toggle, active);
            }
            _brands.SelectionChanged += (_, __) => UpdateToggleText();
            actions.Controls.AddRange(new Control[] { add, rename, toggle });
            root.Controls.Add(_brands, 0, 1);
            tab.Controls.Add(root);
            return tab;
        }

        private TabPage BuildMovements()
        {
            var tab = new TabPage("Stock Movements");
            var root = RootWithActions(out var actions);
            var refresh = new Button { Text = "Refresh" };
            refresh.Click += (_, __) => LoadMovements();
            actions.Controls.Add(refresh);
            actions.Controls.Add(new Label
            {
                Text = "Stock receipts, invoice sales, and void restorations are recorded automatically.",
                AutoSize = true, Padding = new Padding(10, 7, 0, 0)
            });
            root.Controls.Add(_movements, 0, 1);
            tab.Controls.Add(root);
            return tab;
        }

        private TabPage BuildReports()
        {
            var tab = new TabPage("Reports");
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(12) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            actions.Controls.Add(new Label { Text = "From", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
            actions.Controls.Add(_from);
            actions.Controls.Add(new Label { Text = "To", AutoSize = true, Padding = new Padding(8, 7, 0, 0) });
            actions.Controls.Add(_to);
            var today = new Button { Text = "Today" };
            today.Click += (_, __) => { _from.Value = DateTime.Today; _to.Value = DateTime.Today; GenerateReport(); };
            var month = new Button { Text = "This month" };
            month.Click += (_, __) => { _from.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); _to.Value = DateTime.Today; GenerateReport(); };
            var week = new Button { Text = "This week" };
            week.Click += (_, __) =>
            {
                var offset = ((int)DateTime.Today.DayOfWeek + 6) % 7;
                _from.Value = DateTime.Today.AddDays(-offset); _to.Value = DateTime.Today; GenerateReport();
            };
            var generate = new Button { Text = "Refresh report" };
            generate.Click += (_, __) => GenerateReport();
            var print = new Button { Text = "Print report" };
            print.Click += (_, __) => PrintReport();
            actions.Controls.AddRange(new Control[] { today, week, month, generate, print });
            root.Controls.Add(actions, 0, 0);
            root.Controls.Add(_summary, 0, 1);
            var detail = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, BackColor = UiTheme.Border };
            detail.SizeChanged += (_, __) =>
            {
                var stacked = detail.Width < 760;
                detail.Orientation = stacked ? Orientation.Horizontal : Orientation.Vertical;
                if (stacked && detail.Height > 280) detail.SplitterDistance = Math.Max(150, detail.Height / 2);
                else if (!stacked && detail.Width > 360) detail.SplitterDistance = Math.Max(220, detail.Width * 2 / 3);
            };
            detail.Panel1.Padding = new Padding(0, 6, 6, 0); detail.Panel2.Padding = new Padding(6, 6, 0, 0);
            detail.Panel1.Controls.Add(_report); detail.Panel2.Controls.Add(_breakdown);
            root.Controls.Add(detail, 0, 2);
            tab.Controls.Add(root);
            return tab;
        }

        private TabPage BuildActivity()
        {
            var tab = new TabPage("Activity Log");
            var root = RootWithActions(out var actions);
            var refresh = new Button { Text = "Refresh" };
            refresh.Click += (_, __) => LoadActivity();
            actions.Controls.Add(refresh);
            root.Controls.Add(_activity, 0, 1);
            tab.Controls.Add(root);
            return tab;
        }

        private void LoadBrands() => _brands.DataSource = Query(@"
SELECT b.id,b.name,b.is_active,b.sort_order,b.created_at,
       COUNT(DISTINCT p.id) AS products
FROM brands b LEFT JOIN products p ON UPPER(p.brand)=UPPER(b.name)
GROUP BY b.id ORDER BY b.sort_order,b.name;");

        private void LoadMovements()
        {
            _movements.DataSource = Query(@"
SELECT sm.id,sm.created_at,sm.movement_type,p.brand,p.type,COALESCE(p.color,'') AS color,
       sm.quantity_change,sm.quantity_before,sm.quantity_after,COALESCE(sm.note,'') AS note,
       COALESCE(sm.created_by_username,'') AS created_by
FROM stock_movements sm JOIN products p ON p.id=sm.product_id
ORDER BY datetime(sm.created_at) DESC,sm.id DESC;");
            if (_movements.Columns.Contains("id")) _movements.Columns["id"].Visible = false;
        }

        private void LoadActivity()
        {
            _activity.DataSource = Query(@"
SELECT id,created_at,COALESCE(actor_username,'SYSTEM') AS actor,action,entity,
       COALESCE(CAST(entity_id AS TEXT),'') AS entity_id,COALESCE(detail,'') AS detail
FROM audit_log ORDER BY datetime(created_at) DESC,id DESC;");
            if (_activity.Columns.Contains("id")) _activity.Columns["id"].Visible = false;
        }

        private void GenerateReport()
        {
            if (_to.Value.Date < _from.Value.Date)
            {
                MessageBox.Show("The end date must be on or after the start date.");
                return;
            }
            var from = _from.Value.Date.ToString("yyyy-MM-dd 00:00:00");
            var to = _to.Value.Date.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
            using var conn = Database.OpenConnection();
            decimal revenue = Scalar(conn, @"SELECT COALESCE(SUM(ii.line_total),0) FROM invoice_items ii JOIN invoices i ON i.id=ii.invoice_id WHERE i.status='ACTIVE' AND datetime(i.created_at)>=datetime($from) AND datetime(i.created_at)<datetime($to);", from, to);
            decimal service = Scalar(conn, @"SELECT COALESCE(SUM(service_cost),0) FROM services WHERE UPPER(COALESCE(service_status,'RECEIVED'))<>'CANCELLED' AND datetime(COALESCE(created_at,date_time))>=datetime($from) AND datetime(COALESCE(created_at,date_time))<datetime($to);", from, to);
            decimal cost = Scalar(conn, @"SELECT COALESCE(SUM(sl.qty_sold*sl.unit_cost),0) FROM sale_lines sl JOIN sales s ON s.id=sl.sale_id WHERE s.voided=0 AND datetime(s.date_time)>=datetime($from) AND datetime(s.date_time)<datetime($to);", from, to);
            decimal stockIn = Scalar(conn, @"SELECT COALESCE(SUM(CASE WHEN quantity_change>0 THEN quantity_change ELSE 0 END),0) FROM stock_movements WHERE LOWER(movement_type)='stock_in' AND datetime(created_at)>=datetime($from) AND datetime(created_at)<datetime($to);", from, to);
            decimal stockOut = Scalar(conn, @"SELECT COALESCE(ABS(SUM(CASE WHEN quantity_change<0 THEN quantity_change ELSE 0 END)),0) FROM stock_movements WHERE datetime(created_at)>=datetime($from) AND datetime(created_at)<datetime($to);", from, to);
            _reportSummary = $"Period: {_from.Value:dd MMM yyyy} – {_to.Value:dd MMM yyyy}    Sales: {StoreFormat.Money(revenue)}    Service: {StoreFormat.Money(service)}    Gross profit: {StoreFormat.Money(revenue - cost)}    Stock in/out: {stockIn:N0}/{stockOut:N0}";
            _summary.Text = _reportSummary;

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
WITH sale_days AS (
 SELECT date(i.created_at) day,SUM(ii.line_total) sales
 FROM invoices i JOIN invoice_items ii ON ii.invoice_id=i.id
 WHERE i.status='ACTIVE' AND datetime(i.created_at)>=datetime($from) AND datetime(i.created_at)<datetime($to)
 GROUP BY date(i.created_at)
), service_days AS (
 SELECT date(COALESCE(created_at,date_time)) day,SUM(service_cost) service
 FROM services WHERE UPPER(COALESCE(service_status,'RECEIVED'))<>'CANCELLED'
 AND datetime(COALESCE(created_at,date_time))>=datetime($from) AND datetime(COALESCE(created_at,date_time))<datetime($to)
 GROUP BY date(COALESCE(created_at,date_time))
)
SELECT COALESCE(s.day,v.day) AS date,COALESCE(s.sales,0) AS sales,
       COALESCE(v.service,0) AS service,COALESCE(s.sales,0)+COALESCE(v.service,0) AS total
FROM sale_days s LEFT JOIN service_days v ON v.day=s.day
UNION
SELECT v.day,0,v.service,v.service FROM service_days v LEFT JOIN sale_days s ON s.day=v.day WHERE s.day IS NULL
ORDER BY date DESC;";
            cmd.Parameters.AddWithValue("$from", from);
            cmd.Parameters.AddWithValue("$to", to);
            using var reader = cmd.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            _report.DataSource = table;
            foreach (var name in new[] { "sales", "service", "total" })
                if (_report.Columns.Contains(name))
                {
                    _report.Columns[name].DefaultCellStyle.Format = "C0";
                    _report.Columns[name].DefaultCellStyle.FormatProvider = StoreFormat.Culture;
                }

            using var breakdown = conn.CreateCommand();
            breakdown.CommandText = @"
SELECT section,label,value FROM (
 SELECT 'Payment' section,COALESCE(NULLIF(i.payment_method,''),'UNSPECIFIED') label,SUM(ii.line_total) value
 FROM invoices i JOIN invoice_items ii ON ii.invoice_id=i.id
 WHERE i.status='ACTIVE' AND datetime(i.created_at)>=datetime($from) AND datetime(i.created_at)<datetime($to)
 GROUP BY i.payment_method
 UNION ALL
 SELECT 'Top brand',ii.brand,SUM(ii.line_total) FROM invoices i JOIN invoice_items ii ON ii.invoice_id=i.id
 WHERE i.status='ACTIVE' AND datetime(i.created_at)>=datetime($from) AND datetime(i.created_at)<datetime($to) GROUP BY ii.brand
 UNION ALL
 SELECT 'Top model',ii.type,SUM(ii.quantity) FROM invoices i JOIN invoice_items ii ON ii.invoice_id=i.id
 WHERE i.status='ACTIVE' AND datetime(i.created_at)>=datetime($from) AND datetime(i.created_at)<datetime($to) GROUP BY ii.type
 UNION ALL
 SELECT 'Top colour',COALESCE(NULLIF(ii.color,''),'NO COLOUR'),SUM(ii.quantity) FROM invoices i JOIN invoice_items ii ON ii.invoice_id=i.id
 WHERE i.status='ACTIVE' AND datetime(i.created_at)>=datetime($from) AND datetime(i.created_at)<datetime($to) GROUP BY ii.color
) ORDER BY section,value DESC;";
            breakdown.Parameters.AddWithValue("$from", from); breakdown.Parameters.AddWithValue("$to", to);
            using var breakdownReader = breakdown.ExecuteReader(); var breakdownTable = new DataTable(); breakdownTable.Load(breakdownReader); _breakdown.DataSource = breakdownTable;
            if (_breakdown.Columns.Contains("value")) _breakdown.Columns["value"].DefaultCellStyle.Format = "N0";
        }

        private void AddBrand()
        {
            var name = DialogPrompt.Show(this, "Add brand", "Brand name:");
            if (string.IsNullOrWhiteSpace(name)) return;
            ExecuteAdmin("INSERT INTO brands(name) VALUES ($value);", name.ToUpperInvariant(), "CREATE_BRAND", $"name={name}");
            LoadBrands();
        }

        private void RenameBrand()
        {
            if (!SelectedBrand(out var id, out var oldName)) return;
            var name = DialogPrompt.Show(this, "Rename brand", "New brand name:", oldName);
            if (string.IsNullOrWhiteSpace(name) || name.Equals(oldName, StringComparison.OrdinalIgnoreCase)) return;
            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = "UPDATE brands SET name=$name,updated_at=$at WHERE id=$id; UPDATE products SET brand=$name WHERE UPPER(brand)=UPPER($old);";
                    cmd.Parameters.AddWithValue("$name", name.ToUpperInvariant());
                    cmd.Parameters.AddWithValue("$old", oldName);
                    cmd.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("o"));
                    cmd.Parameters.AddWithValue("$id", id);
                    cmd.ExecuteNonQuery();
                }
                LocalAdminRepository.WriteAudit(conn, tx, "RENAME_BRAND", "brands", id, $"{oldName} -> {name.ToUpperInvariant()}");
                tx.Commit();
                LoadBrands();
            }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show(ex.Message); }
        }

        private void ToggleBrand()
        {
            if (!SelectedBrand(out var id, out var name)) return;
            ExecuteAdmin("UPDATE brands SET is_active=CASE is_active WHEN 1 THEN 0 ELSE 1 END,updated_at=$at WHERE id=$id;",
                null, "TOGGLE_BRAND", $"name={name}", id);
            LoadBrands();
        }

        private bool SelectedBrand(out int id, out string name)
        {
            id = 0; name = "";
            if (_brands.CurrentRow?.DataBoundItem is not DataRowView row)
            {
                MessageBox.Show("Select a brand first."); return false;
            }
            id = Convert.ToInt32(row["id"]); name = Convert.ToString(row["name"]) ?? ""; return true;
        }

        private void ExecuteAdmin(string sql, string? value, string action, string detail, int? id = null)
        {
            using var conn = Database.OpenConnection();
            using var tx = conn.BeginTransaction();
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx; cmd.CommandText = sql;
                if (sql.Contains("$value")) cmd.Parameters.AddWithValue("$value", value ?? "");
                if (sql.Contains("$at")) cmd.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("o"));
                if (sql.Contains("$id")) cmd.Parameters.AddWithValue("$id", id ?? 0);
                cmd.ExecuteNonQuery();
                LocalAdminRepository.WriteAudit(conn, tx, action, "brands", id, detail);
                tx.Commit();
            }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show(ex.Message, "Brand update failed"); }
        }

        private void PrintReport()
        {
            GenerateReport();
            var doc = new PrintDocument { DocumentName = "Bike Store Report" };
            doc.DefaultPageSettings.Landscape = true;
            doc.PrintPage += (_, e) =>
            {
                if (e.Graphics == null) return;
                var y = e.MarginBounds.Top;
                using var title = new Font("Segoe UI", 16, FontStyle.Bold);
                using var body = new Font("Segoe UI", 9);
                e.Graphics.DrawString(StoreFormat.ReportHeader, title, Brushes.Black, e.MarginBounds.Left, y);
                y += 35;
                e.Graphics.DrawString(_reportSummary, body, Brushes.Black, new RectangleF(e.MarginBounds.Left, y, e.MarginBounds.Width, 50));
                y += 55;
                foreach (DataGridViewRow row in _report.Rows)
                {
                    if (row.IsNewRow) continue;
                    var line = $"{row.Cells["date"].Value}    Sales: {StoreFormat.Money(Convert.ToDecimal(row.Cells["sales"].Value))}    Service: {StoreFormat.Money(Convert.ToDecimal(row.Cells["service"].Value))}    Total: {StoreFormat.Money(Convert.ToDecimal(row.Cells["total"].Value))}";
                    e.Graphics.DrawString(line, body, Brushes.Black, e.MarginBounds.Left, y);
                    y += 20;
                }
            };
            using var preview = new PrintPreviewDialog { Document = doc, Width = 1050, Height = 750 };
            preview.ShowDialog(this);
        }

        private static DataTable Query(string sql)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand(); cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader(); var table = new DataTable(); table.Load(reader); return table;
        }

        private static decimal Scalar(SqliteConnection conn, string sql, string from, string to)
        {
            using var cmd = conn.CreateCommand(); cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$from", from); cmd.Parameters.AddWithValue("$to", to);
            return Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
        }

        private static TableLayoutPanel RootWithActions(out FlowLayoutPanel actions)
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, Padding = new Padding(12) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoScroll = true, WrapContents = true };
            root.Controls.Add(actions, 0, 0); return root;
        }

        private static DataGridView Grid() => new()
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false
        };
    }
}
