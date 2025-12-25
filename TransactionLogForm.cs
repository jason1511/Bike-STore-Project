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
    id,
    brand,
    type,
    color,
    quantity,
    price,
    customer_name,
    date_time,
    voided
FROM sales
ORDER BY date_time DESC;";
                }
                else
                {
                    cmd.CommandText = @"
SELECT
    id,
    brand,
    type,
    color,
    quantity,
    price,
    customer_name,
    date_time,
    voided
FROM sales
WHERE brand LIKE $q
   OR type LIKE $q
   OR color LIKE $q
   OR customer_name LIKE $q
ORDER BY date_time DESC;";
                    cmd.Parameters.AddWithValue("$q", $"%{search.Trim().ToUpperInvariant()}%");
                }

                using var reader = cmd.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                dgvSales.DataSource = table;

                ApplyColumnPolish();
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

            // IDR formatting for price (no decimals)
            if (dgvSales.Columns.Contains("price"))
            {
                var col = dgvSales.Columns["price"];
                col.DefaultCellStyle.Format = "C0";
                col.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }

            // Optional: hide voided if you aren't using it yet
            // if (dgvSales.Columns.Contains("voided"))
            //     dgvSales.Columns["voided"].Visible = false;
        }
    }
}
