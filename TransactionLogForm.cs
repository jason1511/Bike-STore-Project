using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Bike_STore_Project
{
    public partial class TransactionLogForm : Form
    {
        public TransactionLogForm()
        {
            InitializeComponent();
            SetupGrid();

            Load += TransactionLogForm_Load;
            btnRefresh.Click += (s, e) => LoadData();
        }

        private void TransactionLogForm_Load(object? sender, EventArgs e)
        {
            LoadData();
        }

        private void SetupGrid()
        {
            dgvSales.ReadOnly = true;
            dgvSales.AllowUserToAddRows = false;
            dgvSales.AllowUserToDeleteRows = false;
            dgvSales.MultiSelect = false;
            dgvSales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSales.AutoGenerateColumns = true; // easiest for logs
        }

        private void LoadData()
        {
            try
            {
                using var conn = Database.OpenConnection();
                using var cmd = conn.CreateCommand();

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

                using var reader = cmd.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                dgvSales.DataSource = table;

                // Optional: nicer headers
                dgvSales.Columns["customer_name"].HeaderText = "Customer";
                dgvSales.Columns["date_time"].HeaderText = "Date/Time";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load sales log: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
