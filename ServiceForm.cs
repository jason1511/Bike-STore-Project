using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class ServiceForm : Form
    {
        private void LoadParts()
        {
            cmbPartUsed.Items.Clear();
            cmbPartUsed.Items.Add("None");

            using (var connection = new SqliteConnection("Data Source=data.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT part_name FROM parts ORDER BY part_name";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cmbPartUsed.Items.Add(reader.GetString(0));
                    }
                }
            }
            cmbPartUsed.SelectedIndex = 0; // Default to "None"
        }

        private void ServiceForm_Load(object sender, EventArgs e)
        {
            Database.Initialize(); // make sure tables exist
            LoadParts(); // populate the dropdown
        }

        public ServiceForm()
        {
            InitializeComponent();
        }

        private void btnAddService_Click(object sender, EventArgs e)
        {
            try
            {
                int qtyUsed = Convert.ToInt32(txtPartQty.Text);

                using (var connection = new SqliteConnection("Data Source=data.db"))
                {
                    connection.Open();

                    // Insert service record
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                INSERT INTO services (bike_serial, service_type, part_replaced, part_quantity, service_cost)
                VALUES ($serial, $type, $part, $qty, $cost);
            ";

                    command.Parameters.AddWithValue("$serial", txtBikeSerial.Text);
                    command.Parameters.AddWithValue("$type", txtService.Text);
                    command.Parameters.AddWithValue("$part", cmbPartUsed.SelectedItem?.ToString() ?? "None");
                    command.Parameters.AddWithValue("$qty", qtyUsed);
                    command.Parameters.AddWithValue("$cost", Convert.ToDecimal(txtCost.Text));

                    command.ExecuteNonQuery();

                    // Subtract from parts table (only if not "None")
                    if (cmbPartUsed.SelectedItem?.ToString() != "None")
                    {
                        var updatePartCmd = connection.CreateCommand();
                        updatePartCmd.CommandText = @"
                    UPDATE parts
                    SET quantity = quantity - $usedQty,
                        last_updated = CURRENT_TIMESTAMP
                    WHERE part_name = $part;
                ";

                        updatePartCmd.Parameters.AddWithValue("$usedQty", qtyUsed);
                        updatePartCmd.Parameters.AddWithValue("$part", cmbPartUsed.SelectedItem.ToString());
                        updatePartCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Service recorded and inventory updated.");

                // Clear form fields
                txtBikeSerial.Clear();
                txtService.Clear();
                txtCost.Clear();
                txtPartQty.Text = "0";
                cmbPartUsed.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

    }
}
