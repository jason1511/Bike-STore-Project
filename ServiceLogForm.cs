using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class ServiceLogForm : Form
    {
        public ServiceLogForm()
        {
            InitializeComponent();
            SetupGrid();

            Load += (s, e) => LoadData();
            btnRefresh.Click += (s, e) => LoadData(txtSearch.Text);
            txtSearch.TextChanged += (s, e) => LoadData(txtSearch.Text);
        }

        private void SetupGrid()
        {
            dgvServices.ReadOnly = true;
            dgvServices.AllowUserToAddRows = false;
            dgvServices.AllowUserToDeleteRows = false;
            dgvServices.MultiSelect = false;
            dgvServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvServices.AutoGenerateColumns = true;
            dgvServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvServices.RowHeadersVisible = false;
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
SELECT id, brand, type, color, quantity, service_cost, notes, date_time
FROM services
ORDER BY date_time DESC;";
                }
                else
                {
                    cmd.CommandText = @"
SELECT id, brand, type, color, quantity, service_cost, notes, date_time
FROM services
WHERE brand LIKE $q OR type LIKE $q OR color LIKE $q OR notes LIKE $q
ORDER BY date_time DESC;";
                    cmd.Parameters.AddWithValue("$q", $"%{search.Trim().ToUpperInvariant()}%");
                }

                using var reader = cmd.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                dgvServices.DataSource = table;

                ApplyColumnPolish();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load service log: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyColumnPolish()
        {
            if (dgvServices.Columns.Count == 0) return;

            // Hide ID
            if (dgvServices.Columns.Contains("id"))
                dgvServices.Columns["id"].Visible = false;

            // Friendly headers
            if (dgvServices.Columns.Contains("service_cost"))
                dgvServices.Columns["service_cost"].HeaderText = "Cost";
            if (dgvServices.Columns.Contains("date_time"))
                dgvServices.Columns["date_time"].HeaderText = "Date/Time";

            // IDR formatting (no decimals)
            if (dgvServices.Columns.Contains("service_cost"))
            {
                var col = dgvServices.Columns["service_cost"];
                col.DefaultCellStyle.Format = "C0";
                col.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }

            // Notes wider + wrap
            if (dgvServices.Columns.Contains("notes"))
            {
                dgvServices.Columns["notes"].HeaderText = "Notes";
                dgvServices.Columns["notes"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvServices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }

            // Quantity is always 1 now, you can hide it if you want:
            // if (dgvServices.Columns.Contains("quantity"))
            //     dgvServices.Columns["quantity"].Visible = false;
        }
    }
}
