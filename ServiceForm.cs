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
    public partial class ServiceForm : Form
    {
        public ServiceForm()
        {
            InitializeComponent();

            btnAddService.Click += BtnAddService_Click;
            btnClear.Click += (s, e) => ClearInputs();
        }

        private void BtnAddService_Click(object? sender, EventArgs e)
        {
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
            var cost = numServiceCost.Value;

            try
            {
                using var conn = Database.OpenConnection();
                using var cmd = conn.CreateCommand();

                cmd.CommandText = @"
INSERT INTO services (brand, type, color, quantity, service_cost, notes)
VALUES ($brand, $type, $color, $qty, $cost, $notes);";

                cmd.Parameters.AddWithValue("$brand", txtBrand.Text.Trim());
                cmd.Parameters.AddWithValue("$type", txtType.Text.Trim());
                cmd.Parameters.AddWithValue("$color", string.IsNullOrWhiteSpace(txtColor.Text) ? (object)DBNull.Value : txtColor.Text.Trim());
                cmd.Parameters.AddWithValue("$qty", qty);
                cmd.Parameters.AddWithValue("$cost", (double)cost);
                cmd.Parameters.AddWithValue("$notes", string.IsNullOrWhiteSpace(txtNotes.Text) ? (object)DBNull.Value : txtNotes.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("Service saved!");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save service: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtBrand.Clear();
            txtType.Clear();
            txtColor.Clear();
            numQuantity.Value = 1;
            numServiceCost.Value = 0;
            txtNotes.Clear();
            txtBrand.Focus();
        }
    }
}

