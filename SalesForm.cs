using Bike_STore_Project;
using Microsoft.Data.Sqlite;

namespace WinFormsApp1
{
    public partial class SalesForm : Form
    {
        public SalesForm()
        {
            InitializeComponent();
        }

        private void btnAddSale_Click(object sender, EventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtPrice.Text, out var price))
                {
                    MessageBox.Show("Please enter a valid price.");
                    return;
                }

                using var connection = Database.OpenConnection();
                using var command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO sales (bike_model, bike_serial, price, customer_name)
            VALUES ($model, $serial, $price, $customer);";

                command.Parameters.AddWithValue("$model", txtBikeModel.Text.Trim());
                command.Parameters.AddWithValue("$serial", txtBikeSerial.Text.Trim());
                command.Parameters.AddWithValue("$price", price);
                command.Parameters.AddWithValue("$customer", txtCustomerName.Text.Trim());

                command.ExecuteNonQuery();

                MessageBox.Show("Sale added successfully!");
                txtBikeModel.Clear();
                txtBikeSerial.Clear();
                txtPrice.Clear();
                txtCustomerName.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void salesFormToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var serviceForm = new ServiceForm();
            serviceForm.Show();
        }

        private void salesFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var inv = new InventoryForm())
    {
                inv.ShowDialog(this);
            }
        }
    }
}
