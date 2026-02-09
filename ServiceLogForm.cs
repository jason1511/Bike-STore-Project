using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class ServiceLogForm : Form
    {
        public ServiceLogForm()
        {
            InitializeComponent();
            SetupGrid();

            txtSearch.CharacterCasing = CharacterCasing.Upper;

            // ✅ Title updates after logout/login
            ApplyWindowTitle();
            Shown += (_, __) => ApplyWindowTitle();
            Activated += (_, __) => ApplyWindowTitle();

            Load += (s, e) => LoadData();
            btnRefresh.Click += (s, e) => LoadData(txtSearch.Text);
            txtSearch.TextChanged += (s, e) => LoadData(txtSearch.Text);
        }

        private void ApplyWindowTitle()
        {
            var who = AppSession.IsSignedIn
                ? $"{AppSession.Username} ({AppSession.Role})"
                : "Not signed in";

            Text = $"Bike Store - Service Log - {who}";
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

                // ✅ Use created_at if present, otherwise fallback to legacy date_time
                //    and expose it as "date_time" for your existing UI polish code
                if (string.IsNullOrWhiteSpace(search))
                {
                    cmd.CommandText = @"
SELECT
    id,
    brand,
    type,
    color,
    quantity,
    service_cost,
    notes,
    COALESCE(created_at, date_time) AS date_time,
    COALESCE(created_by_username, '') AS created_by_username
FROM services
ORDER BY datetime(COALESCE(created_at, date_time)) DESC, id DESC;";
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
    service_cost,
    notes,
    COALESCE(created_at, date_time) AS date_time,
    COALESCE(created_by_username, '') AS created_by_username
FROM services
WHERE brand LIKE $q
   OR type LIKE $q
   OR COALESCE(color,'') LIKE $q
   OR COALESCE(notes,'') LIKE $q
   OR COALESCE(created_by_username,'') LIKE $q
ORDER BY datetime(COALESCE(created_at, date_time)) DESC, id DESC;";
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

            if (dgvServices.Columns.Contains("created_by_username"))
                dgvServices.Columns["created_by_username"].HeaderText = "By";

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
        }
    }
}
