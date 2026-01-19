using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class TransactionLogForm : Form
    {
        public TransactionLogForm()
        {
            InitializeComponent();
            SetupGrid();

            txtSearch.CharacterCasing = CharacterCasing.Upper;

            Load += (s, e) => LoadData();
            btnRefresh.Click += (s, e) => LoadData(txtSearch.Text);
            txtSearch.TextChanged += (s, e) => LoadData(txtSearch.Text);
        }

        private void SetupGrid()
        {
            dgvSales.ReadOnly = true;
            dgvSales.AllowUserToAddRows = false;
            dgvSales.AllowUserToDeleteRows = false;
            dgvSales.MultiSelect = false;
            dgvSales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSales.AutoGenerateColumns = true;
            dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSales.RowHeadersVisible = false;

            // when a sale row is selected, load its FIFO lines
            dgvSales.SelectionChanged += (s, e) => LoadSelectedSaleLines();

            dgvSaleLines.ReadOnly = true;
            dgvSaleLines.AllowUserToAddRows = false;
            dgvSaleLines.AllowUserToDeleteRows = false;
            dgvSaleLines.MultiSelect = false;
            dgvSaleLines.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSaleLines.AutoGenerateColumns = true;
            dgvSaleLines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSaleLines.RowHeadersVisible = false;
        }

        private void LoadSaleLines(int saleId)
        {
            try
            {
                using var conn = Database.OpenConnection();
                using var cmd = conn.CreateCommand();

                cmd.CommandText = @"
SELECT
    sl.stock_lot_id AS lot_id,
    sl.qty_sold,
    sl.unit_cost,
    sl.unit_sell,
    (sl.qty_sold * sl.unit_cost) AS line_cost,
    (sl.qty_sold * sl.unit_sell) AS line_revenue,
    (sl.qty_sold * (sl.unit_sell - sl.unit_cost)) AS line_profit
FROM sale_lines sl
WHERE sl.sale_id = $saleId
ORDER BY sl.stock_lot_id ASC;";
                cmd.Parameters.AddWithValue("$saleId", saleId);

                using var reader = cmd.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                dgvSaleLines.DataSource = table;

                foreach (var name in new[] { "unit_cost", "unit_sell", "line_cost", "line_revenue", "line_profit" })
                {
                    if (dgvSaleLines.Columns.Contains(name))
                    {
                        var col = dgvSaleLines.Columns[name];
                        col.DefaultCellStyle.Format = "C0";
                        col.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load sale lines: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSelectedSaleLines()
        {
            if (dgvSales.CurrentRow?.DataBoundItem is not DataRowView row) return;
            if (!row.DataView.Table.Columns.Contains("id")) return;

            int saleId = Convert.ToInt32(row["id"]);
            LoadSaleLines(saleId);
        }

        private void LoadData(string? search = null)
        {
            try
            {
                using var conn = Database.OpenConnection();
                using var cmd = conn.CreateCommand();

                if (string.IsNullOrWhiteSpace(search))
                {
                    cmd.CommandText = @"
SELECT
    s.id,
    s.brand,
    s.type,
    s.color,
    s.customer_name,
    s.date_time,
    s.voided,
    COALESCE(SUM(sl.qty_sold), 0) AS qty_sold,
    COALESCE(SUM(sl.qty_sold * sl.unit_sell), 0) AS revenue,
    COALESCE(SUM(sl.qty_sold * sl.unit_cost), 0) AS cost,
    COALESCE(SUM(sl.qty_sold * (sl.unit_sell - sl.unit_cost)), 0) AS profit
FROM sales s
LEFT JOIN sale_lines sl ON sl.sale_id = s.id
GROUP BY s.id, s.brand, s.type, s.color, s.customer_name, s.date_time, s.voided
ORDER BY datetime(s.date_time) DESC, s.id DESC;";
                }
                else
                {
                    cmd.CommandText = @"
SELECT
    s.id,
    s.brand,
    s.type,
    s.color,
    s.customer_name,
    s.date_time,
    s.voided,
    COALESCE(SUM(sl.qty_sold), 0) AS qty_sold,
    COALESCE(SUM(sl.qty_sold * sl.unit_sell), 0) AS revenue,
    COALESCE(SUM(sl.qty_sold * sl.unit_cost), 0) AS cost,
    COALESCE(SUM(sl.qty_sold * (sl.unit_sell - sl.unit_cost)), 0) AS profit
FROM sales s
LEFT JOIN sale_lines sl ON sl.sale_id = s.id
WHERE s.brand LIKE $q
   OR s.type LIKE $q
   OR COALESCE(s.color,'') LIKE $q
   OR COALESCE(s.customer_name,'') LIKE $q
GROUP BY s.id, s.brand, s.type, s.color, s.customer_name, s.date_time, s.voided
ORDER BY datetime(s.date_time) DESC, s.id DESC;";
                    cmd.Parameters.AddWithValue("$q", $"%{search.Trim().ToUpperInvariant()}%");
                }

                using var reader = cmd.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                dgvSales.DataSource = table;
                ApplyColumnPolish();

                // select first row AFTER binding (so your sale_lines auto-load)
                if (dgvSales.Rows.Count > 0)
                {
                    dgvSales.ClearSelection();
                    dgvSales.Rows[0].Selected = true;

                    var firstVisible = dgvSales.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                    if (firstVisible != null)
                        dgvSales.CurrentCell = dgvSales.Rows[0].Cells[firstVisible.Index];
                }
                else
                {
                    dgvSaleLines.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load sales log: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyColumnPolish()
        {
            if (dgvSales.Columns.Count == 0) return;

            // Hide ID
            if (dgvSales.Columns.Contains("id"))
                dgvSales.Columns["id"].Visible = false;

            // Friendly headers
            if (dgvSales.Columns.Contains("customer_name"))
                dgvSales.Columns["customer_name"].HeaderText = "Customer";
            if (dgvSales.Columns.Contains("date_time"))
                dgvSales.Columns["date_time"].HeaderText = "Date/Time";

            // Currency formatting (IDR) for revenue/cost/profit
            foreach (var name in new[] { "revenue", "cost", "profit" })
            {
                if (dgvSales.Columns.Contains(name))
                {
                    var col = dgvSales.Columns[name];
                    col.DefaultCellStyle.Format = "C0";
                    col.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
                }
            }

            // Make headers nicer
            if (dgvSales.Columns.Contains("qty_sold"))
                dgvSales.Columns["qty_sold"].HeaderText = "Qty Sold";
            if (dgvSales.Columns.Contains("revenue"))
                dgvSales.Columns["revenue"].HeaderText = "Revenue";
            if (dgvSales.Columns.Contains("cost"))
                dgvSales.Columns["cost"].HeaderText = "Cost";
            if (dgvSales.Columns.Contains("profit"))
                dgvSales.Columns["profit"].HeaderText = "Profit";
        }
    }
}
