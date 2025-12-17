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
    public partial class SalesForm : Form
    {
        public SalesForm()
        {
            InitializeComponent();

            btnAddSale.Click += BtnAddSale_Click;
            btnClear.Click += (s, e) => ClearInputs();
        }

        private void BtnAddSale_Click(object? sender, EventArgs e)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(txtBrand.Text))
            {
                MessageBox.Show("Brand is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtType.Text))
            {
                MessageBox.Show("Type is required.");
                return;
            }

            var qty = (int)numQuantity.Value;
            var price = numPrice.Value;

            try
            {
                using var conn = Database.OpenConnection();
                using var cmd = conn.CreateCommand();

                cmd.CommandText = @"
INSERT INTO sales (brand, type, color, quantity, price, customer_name)
VALUES ($brand, $type, $color, $qty, $price, $customer);";

                cmd.Parameters.AddWithValue("$brand", txtBrand.Text.Trim());
                cmd.Parameters.AddWithValue("$type", txtType.Text.Trim());
                cmd.Parameters.AddWithValue("$color", string.IsNullOrWhiteSpace(txtColor.Text) ? (object)DBNull.Value : txtColor.Text.Trim());
                cmd.Parameters.AddWithValue("$qty", qty);
                cmd.Parameters.AddWithValue("$price", (double)price);
                cmd.Parameters.AddWithValue("$customer", string.IsNullOrWhiteSpace(txtCustomer.Text) ? (object)DBNull.Value : txtCustomer.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("Sale saved!");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save sale: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtBrand.Clear();
            txtType.Clear();
            txtColor.Clear();
            numQuantity.Value = 1;
            numPrice.Value = 0;
            txtCustomer.Clear();
            txtBrand.Focus();
        }
    }
}

