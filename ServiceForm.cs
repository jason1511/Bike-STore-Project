using System;
using System.Linq;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class ServiceForm : Form
    {
        private readonly ProductRepository _repo = new();

        public ServiceForm()
        {
            InitializeComponent();

            // uppercase everywhere
            txtBrand.CharacterCasing = CharacterCasing.Upper;
            txtType.CharacterCasing = CharacterCasing.Upper;
            txtColor.CharacterCasing = CharacterCasing.Upper;

            // money-like input
            numServiceCost.DecimalPlaces = 2;
            numServiceCost.Minimum = 0;

            SetupAutocomplete();

            txtBrand.TextChanged += (s, e) =>
            {
                SetupTypeAutocomplete();
                SetupColorAutocomplete();
            };

            txtType.TextChanged += (s, e) =>
            {
                SetupColorAutocomplete();
            };

            btnAddService.Click += BtnAddService_Click;
            btnClear.Click += (s, e) => ClearInputs();
        }

        private void SetupAutocomplete()
        {
            txtBrand.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtBrand.AutoCompleteSource = AutoCompleteSource.CustomSource;

            txtType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtType.AutoCompleteSource = AutoCompleteSource.CustomSource;

            txtColor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtColor.AutoCompleteSource = AutoCompleteSource.CustomSource;

            SetupBrandAutocomplete();
        }

        private void SetupBrandAutocomplete()
        {
            var src = new AutoCompleteStringCollection();
            src.AddRange(_repo.GetDistinctBrands().ToArray());
            txtBrand.AutoCompleteCustomSource = src;
        }

        private void SetupTypeAutocomplete()
        {
            var src = new AutoCompleteStringCollection();

            if (!string.IsNullOrWhiteSpace(txtBrand.Text))
                src.AddRange(_repo.GetDistinctTypes(txtBrand.Text).ToArray());

            txtType.AutoCompleteCustomSource = src;
        }

        private void SetupColorAutocomplete()
        {
            var src = new AutoCompleteStringCollection();

            if (!string.IsNullOrWhiteSpace(txtBrand.Text) &&
                !string.IsNullOrWhiteSpace(txtType.Text))
            {
                src.AddRange(_repo.GetDistinctColors(txtBrand.Text, txtType.Text).ToArray());
            }

            txtColor.AutoCompleteCustomSource = src;
        }

        private void BtnAddService_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBrand.Text))
            {
                MessageBox.Show("Brand is required.");
                txtBrand.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtType.Text))
            {
                MessageBox.Show("Type is required.");
                txtType.Focus();
                return;
            }

            try
            {
                using var conn = Database.OpenConnection();
                using var cmd = conn.CreateCommand();

                cmd.CommandText = @"
INSERT INTO services (brand, type, color, quantity, service_cost, notes)
VALUES ($brand, $type, $color, 1, $cost, $notes);";

                cmd.Parameters.AddWithValue("$brand", txtBrand.Text.Trim().ToUpperInvariant());
                cmd.Parameters.AddWithValue("$type", txtType.Text.Trim().ToUpperInvariant());
                cmd.Parameters.AddWithValue("$color",
                    string.IsNullOrWhiteSpace(txtColor.Text)
                        ? (object)DBNull.Value
                        : txtColor.Text.Trim().ToUpperInvariant());

                cmd.Parameters.AddWithValue("$cost", (double)numServiceCost.Value);
                cmd.Parameters.AddWithValue("$notes",
                    string.IsNullOrWhiteSpace(txtNotes.Text)
                        ? (object)DBNull.Value
                        : txtNotes.Text.Trim());

                cmd.ExecuteNonQuery();

                MessageBox.Show("Service saved!");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save service: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtBrand.Clear();
            txtType.Clear();
            txtColor.Clear();
            txtNotes.Clear();

            // reset safely
            numServiceCost.Value = 0;

            SetupBrandAutocomplete();
            txtBrand.Focus();
        }
    }
}
