using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    internal sealed class DashboardPageControl : UserControl
    {
        private readonly FlowLayoutPanel _metrics = new() { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
        private readonly DataGridView _invoices = Grid();
        private readonly DataGridView _services = Grid();
        private readonly MiniBarChart _salesChart = new() { Dock = DockStyle.Fill, MinimumSize = new Size(300, 180) };
        private readonly MiniBarChart _stockChart = new() { Dock = DockStyle.Fill, MinimumSize = new Size(300, 180) };

        public event Action<string>? NavigateRequested;

        public DashboardPageControl()
        {
            Dock = DockStyle.Fill;
            BackColor = UiTheme.Canvas;
            Padding = new Padding(20);
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 126));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 55));

            var quick = new FlowLayoutPanel { Dock = DockStyle.Fill, Height = 46, AutoSize = true };
            quick.Controls.Add(QuickButton("+ New invoice", "sales", UiTheme.Accent));
            quick.Controls.Add(QuickButton("+ Receive stock", "inventory", UiTheme.Success));
            quick.Controls.Add(QuickButton("+ New service", "service", UiTheme.Warning));
            var refresh = QuickButton("Refresh dashboard", "refresh", UiTheme.SidebarHover);
            refresh.Click -= NavigateClick;
            refresh.Click += (_, __) => RefreshDashboard();
            quick.Controls.Add(refresh);

            var charts = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0, 8, 0, 8) };
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            charts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            charts.Controls.Add(CardWithTitle("Sales — last 7 days", _salesChart), 0, 0);
            charts.Controls.Add(CardWithTitle("Stock movement — last 7 days", _stockChart), 1, 0);

            var recent = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(0, 4, 0, 0) };
            recent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
            recent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            recent.Controls.Add(CardWithTitle("Recent invoices", _invoices), 0, 0);
            recent.Controls.Add(CardWithTitle("Open services", _services), 1, 0);

            root.Controls.Add(quick, 0, 0);
            root.Controls.Add(_metrics, 0, 1);
            root.Controls.Add(charts, 0, 2);
            root.Controls.Add(recent, 0, 3);
            Controls.Add(root);
            Load += (_, __) => RefreshDashboard();
        }

        public void RefreshDashboard()
        {
            try
            {
                _metrics.Controls.Clear();
                using var conn = Database.OpenConnection();
                var today = DateTime.Today.ToString("yyyy-MM-dd");
                var sales = Scalar(conn, "SELECT COALESCE(SUM(ii.line_total),0) FROM invoice_items ii JOIN invoices i ON i.id=ii.invoice_id WHERE i.status='ACTIVE' AND date(i.created_at)=$date;", today);
                var invoiceCount = Scalar(conn, "SELECT COUNT(*) FROM invoices WHERE status='ACTIVE' AND date(created_at)=$date;", today);
                var serviceCount = Scalar(conn, "SELECT COUNT(*) FROM services WHERE UPPER(COALESCE(service_status,'RECEIVED')) IN ('RECEIVED','IN_PROGRESS');", today, false);
                var stock = Scalar(conn, "SELECT COALESCE(SUM(qty_remaining),0) FROM stock_lots;", today, false);
                var lowStock = Scalar(conn, @"SELECT COUNT(*) FROM (SELECT p.id,COALESCE(SUM(l.qty_remaining),0) qty FROM products p LEFT JOIN stock_lots l ON l.product_id=p.id WHERE COALESCE(p.is_active,1)=1 GROUP BY p.id HAVING qty<=2);", today, false);
                var cost = Scalar(conn, "SELECT COALESCE(SUM(sl.qty_sold*sl.unit_cost),0) FROM sale_lines sl JOIN sales s ON s.id=sl.sale_id WHERE s.voided=0 AND date(s.date_time)=$date;", today);

                AddMetric("Today's sales", $"Rp {sales:N0}", UiTheme.Accent);
                AddMetric("Invoices", invoiceCount.ToString("N0"), Color.FromArgb(77, 105, 160));
                AddMetric("Open services", serviceCount.ToString("N0"), UiTheme.Warning);
                AddMetric("Stock units", stock.ToString("N0"), UiTheme.Success);
                AddMetric("Low stock", lowStock.ToString("N0"), UiTheme.Danger);
                AddMetric("Gross profit", $"Rp {sales - cost:N0}", Color.FromArgb(105, 88, 155));

                _invoices.DataSource = Query(conn, @"
SELECT i.invoice_number AS invoice,i.customer_name AS customer,i.payment_method AS payment,
       i.status,COALESCE(SUM(ii.line_total),0) AS total,i.created_at AS created
FROM invoices i LEFT JOIN invoice_items ii ON ii.invoice_id=i.id
GROUP BY i.id ORDER BY datetime(i.created_at) DESC LIMIT 8;");
                if (_invoices.Columns.Contains("total")) FormatMoney(_invoices.Columns["total"]);

                _services.DataSource = Query(conn, @"
SELECT COALESCE(service_number,'LEGACY-'||id) AS service,COALESCE(customer_name,'') AS customer,
       brand||' '||type AS bicycle,COALESCE(service_status,'RECEIVED') AS status,
       COALESCE(created_at,date_time) AS created
FROM services WHERE UPPER(COALESCE(service_status,'RECEIVED')) IN ('RECEIVED','IN_PROGRESS')
ORDER BY datetime(COALESCE(created_at,date_time)) DESC LIMIT 8;");
                LoadCharts(conn);
                UiTheme.Apply(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard could not be refreshed: " + ex.Message, "Dashboard");
            }
        }

        private void LoadCharts(Microsoft.Data.Sqlite.SqliteConnection conn)
        {
            var sales = new List<ChartValue>();
            var stock = new List<ChartValue>();
            for (var day = DateTime.Today.AddDays(-6); day <= DateTime.Today; day = day.AddDays(1))
            {
                var key = day.ToString("yyyy-MM-dd");
                var revenue = Scalar(conn, "SELECT COALESCE(SUM(ii.line_total),0) FROM invoice_items ii JOIN invoices i ON i.id=ii.invoice_id WHERE i.status='ACTIVE' AND date(i.created_at)=$date;", key);
                var stockIn = Scalar(conn, "SELECT COALESCE(SUM(CASE WHEN quantity_change>0 THEN quantity_change ELSE 0 END),0) FROM stock_movements WHERE date(created_at)=$date AND movement_type IN ('STOCK_IN','OPENING_STOCK');", key);
                var stockOut = Scalar(conn, "SELECT COALESCE(ABS(SUM(CASE WHEN quantity_change<0 THEN quantity_change ELSE 0 END)),0) FROM stock_movements WHERE date(created_at)=$date;", key);
                sales.Add(new ChartValue(day.ToString("ddd"), revenue, 0));
                stock.Add(new ChartValue(day.ToString("ddd"), stockIn, stockOut));
            }
            _salesChart.SetData(sales, UiTheme.Accent, Color.Transparent, "Revenue", "");
            _stockChart.SetData(stock, UiTheme.Success, UiTheme.Danger, "In", "Out");
        }

        private Button QuickButton(string text, string destination, Color color)
        {
            var button = new Button { Text = text, Tag = "primary", AccessibleDescription = destination, Height = 36, AutoSize = true, Margin = new Padding(0, 0, 10, 4) };
            button.FlatStyle = FlatStyle.Flat; button.FlatAppearance.BorderSize = 0; button.BackColor = color; button.ForeColor = Color.White;
            button.Cursor = Cursors.Hand; button.Padding = new Padding(12, 0, 12, 0); button.Click += NavigateClick; return button;
        }

        private void NavigateClick(object? sender, EventArgs e)
        {
            if (sender is Button button && !string.IsNullOrWhiteSpace(button.AccessibleDescription))
                NavigateRequested?.Invoke(button.AccessibleDescription);
        }

        private void AddMetric(string label, string value, Color accent)
        {
            _metrics.Controls.Add(new MetricCard(label, value, accent) { Width = 186, Height = 105, Margin = new Padding(0, 8, 12, 8) });
        }

        private static Panel CardWithTitle(string title, Control content)
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Card, Padding = new Padding(14), Margin = new Padding(0, 4, 12, 4), BorderStyle = BorderStyle.FixedSingle };
            var label = new Label { Text = title, Dock = DockStyle.Top, Height = 28, Font = new Font("Segoe UI Semibold", 10F), ForeColor = UiTheme.Text };
            content.Dock = DockStyle.Fill; panel.Controls.Add(content); panel.Controls.Add(label); return panel;
        }

        private static DataGridView Grid() => new()
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false
        };
        private static decimal Scalar(Microsoft.Data.Sqlite.SqliteConnection conn, string sql, string value, bool bindDate = true)
        {
            using var cmd = conn.CreateCommand(); cmd.CommandText = sql;
            if (bindDate) cmd.Parameters.AddWithValue("$date", value);
            return Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
        }
        private static DataTable Query(Microsoft.Data.Sqlite.SqliteConnection conn, string sql)
        {
            using var cmd = conn.CreateCommand(); cmd.CommandText = sql; using var reader = cmd.ExecuteReader(); var table = new DataTable(); table.Load(reader); return table;
        }
        private static void FormatMoney(DataGridViewColumn column)
        {
            column.DefaultCellStyle.Format = "C0"; column.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
        }
    }

    internal sealed class MetricCard : UserControl
    {
        public MetricCard(string label, string value, Color accent)
        {
            Tag = "card"; BackColor = UiTheme.Card; BorderStyle = BorderStyle.FixedSingle; Padding = new Padding(14, 12, 10, 8);
            var strip = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent };
            var caption = new Label { Text = label, Tag = "muted", Dock = DockStyle.Top, Height = 25, ForeColor = UiTheme.Muted, Font = new Font("Segoe UI", 9F) };
            var amount = new Label { Text = value, Dock = DockStyle.Fill, ForeColor = UiTheme.Text, Font = new Font("Segoe UI Semibold", 15F), AutoEllipsis = true };
            Controls.Add(amount); Controls.Add(caption); Controls.Add(strip);
        }
    }

    internal readonly record struct ChartValue(string Label, decimal First, decimal Second);

    internal sealed class MiniBarChart : Control
    {
        private IReadOnlyList<ChartValue> _data = Array.Empty<ChartValue>();
        private Color _first = UiTheme.Accent; private Color _second = UiTheme.Success;
        private string _firstName = ""; private string _secondName = "";
        public MiniBarChart() { DoubleBuffered = true; BackColor = Color.White; }
        public void SetData(IReadOnlyList<ChartValue> data, Color first, Color second, string firstName, string secondName)
        { _data = data; _first = first; _second = second; _firstName = firstName; _secondName = secondName; Invalidate(); }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); if (_data.Count == 0) return;
            var g = e.Graphics; g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var area = new Rectangle(38, 12, Math.Max(10, Width - 52), Math.Max(10, Height - 42));
            using var gridPen = new Pen(UiTheme.Border); g.DrawLine(gridPen, area.Left, area.Bottom, area.Right, area.Bottom);
            var max = Math.Max(1m, _data.Max(x => Math.Max(x.First, x.Second)));
            var slot = area.Width / (float)_data.Count; var barWidth = Math.Max(5, slot * (_second == Color.Transparent ? .48f : .28f));
            using var firstBrush = new SolidBrush(_first); using var secondBrush = new SolidBrush(_second);
            using var textBrush = new SolidBrush(UiTheme.Muted); using var font = new Font("Segoe UI", 8F);
            for (var i = 0; i < _data.Count; i++)
            {
                var item = _data[i]; var center = area.Left + slot * i + slot / 2;
                var firstHeight = (float)(item.First / max) * (area.Height - 8);
                g.FillRectangle(firstBrush, center - barWidth - 1, area.Bottom - firstHeight, barWidth, firstHeight);
                if (_second != Color.Transparent)
                {
                    var secondHeight = (float)(item.Second / max) * (area.Height - 8);
                    g.FillRectangle(secondBrush, center + 1, area.Bottom - secondHeight, barWidth, secondHeight);
                }
                var size = g.MeasureString(item.Label, font); g.DrawString(item.Label, font, textBrush, center - size.Width / 2, area.Bottom + 5);
            }
            var legend = _second == Color.Transparent ? _firstName : $"{_firstName}  •  {_secondName}";
            g.DrawString(legend, font, textBrush, area.Left, 0);
        }
    }
}
