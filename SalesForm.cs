using System;
using System.Data;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class SalesForm : Form
    {
        private const string NoColor = "(NO COLOR)";
        private int _availableQty = 0;
        private int _selectedProductId = 0;

        public SalesForm()
        {
            InitializeComponent();

            btnAddSale.Click += BtnAddSale_Click;
            btnClear.Click += (s, e) => ClearInputs();
            cmbBrand.SelectedIndexChanged += (s, e) => LoadTypes();
            cmbType.SelectedIndexChanged += (s, e) => LoadColors();
            cmbColor.SelectedIndexChanged += (s, e) => UpdateStockAndDefaults();

            numQuantity.Minimum = 1;
            numQuantity.Value = 1;

            LoadBrands();
        }
        

        private void LoadTypes()
        {
            cmbType.Items.Clear();
            cmbColor.Items.Clear();

            if (cmbBrand.SelectedItem == null)
            {
                cmbType.Enabled = false;
                cmbColor.Enabled = false;
                btnAddSale.Enabled = false;
                return;
            }

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT DISTINCT type FROM products WHERE brand = $brand ORDER BY type;";
            cmd.Parameters.AddWithValue("$brand", cmbBrand.SelectedItem.ToString()!);

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                cmbType.Items.Add(rdr.GetString(0));

            cmbType.Enabled = cmbType.Items.Count > 0;
            cmbColor.Enabled = false;
            btnAddSale.Enabled = false;

            if (cmbType.Items.Count > 0)
                cmbType.SelectedIndex = 0;
        }
        private void LoadColors()
        {
            cmbColor.Items.Clear();

            if (cmbBrand.SelectedItem == null || cmbType.SelectedItem == null)
            {
                cmbColor.Enabled = false;
                btnAddSale.Enabled = false;
                return;
            }

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT DISTINCT COALESCE(color, '') AS color
FROM products
WHERE brand = $brand AND type = $type
ORDER BY color;";
            cmd.Parameters.AddWithValue("$brand", cmbBrand.SelectedItem.ToString()!);
            cmd.Parameters.AddWithValue("$type", cmbType.SelectedItem.ToString()!);

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var c = rdr.GetString(0);
                cmbColor.Items.Add(string.IsNullOrWhiteSpace(c) ? NoColor : c);
            }

            cmbColor.Enabled = cmbColor.Items.Count > 0;
            btnAddSale.Enabled = false;

            if (cmbColor.Items.Count > 0)
                cmbColor.SelectedIndex = 0;
        }

        private void LoadBrands()
        {
            cmbBrand.Items.Clear();
            cmbType.Items.Clear();
            cmbColor.Items.Clear();

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT DISTINCT brand FROM products ORDER BY brand;";

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                cmbBrand.Items.Add(rdr.GetString(0));

            cmbBrand.Enabled = cmbBrand.Items.Count > 0;
            cmbType.Enabled = false;
            cmbColor.Enabled = false;
            btnAddSale.Enabled = false;

            if (cmbBrand.Items.Count > 0)
                cmbBrand.SelectedIndex = 0;
        }
        private void UpdateStockAndDefaults()
        {
            btnAddSale.Enabled = false;
            _availableQty = 0;
            _selectedProductId = 0;

            if (cmbBrand.SelectedItem == null || cmbType.SelectedItem == null || cmbColor.SelectedItem == null)
                return;

            var brand = cmbBrand.SelectedItem.ToString()!;
            var type = cmbType.SelectedItem.ToString()!;
            var colorText = cmbColor.SelectedItem.ToString()!;
            string? color = (colorText == NoColor) ? null : colorText;

            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT
  p.id,
  COALESCE(SUM(l.qty_remaining),0) AS available_qty
FROM products p
LEFT JOIN stock_lots l ON l.product_id = p.id AND l.qty_remaining > 0
WHERE p.brand = $brand
  AND p.type  = $type
  AND ((p.color IS NULL AND $color IS NULL) OR (p.color = $color))
GROUP BY p.id
LIMIT 1;";

            cmd.Parameters.AddWithValue("$brand", brand);
            cmd.Parameters.AddWithValue("$type", type);
            cmd.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);

            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read())
            {
                _selectedProductId = 0;
                _availableQty = 0;
                numQuantity.Maximum = 1;
                numQuantity.Value = 1;
                numPrice.Enabled = false;
                btnAddSale.Enabled = false;
                return;

            }

            _selectedProductId = rdr.GetInt32(0);
            _availableQty = rdr.GetInt32(1);

            numQuantity.Maximum = Math.Max(1, _availableQty);
            if (numQuantity.Value < 1) numQuantity.Value = 1;
            if (numQuantity.Value > numQuantity.Maximum) numQuantity.Value = numQuantity.Maximum;

            numPrice.Enabled = _availableQty > 0;
            btnAddSale.Enabled = _availableQty > 0;
        }

        private void BtnAddSale_Click(object? sender, EventArgs e)
        {
            if (cmbBrand.SelectedItem == null || cmbType.SelectedItem == null || cmbColor.SelectedItem == null)
            {
                MessageBox.Show("Select Brand, Type, and Color first.");
                return;
            }

            if (numPrice.Value <= 0)
            {
                MessageBox.Show("Enter a selling price.");
                return;
            }

            if (_selectedProductId == 0)
            {
                MessageBox.Show("No product selected.");
                return;
            }

            var repo = new ProductRepository();

            var ok = repo.TryMakeSaleFifoManualPrice(
                productId: _selectedProductId,
                saleQty: (int)numQuantity.Value,
                saleUnitPrice: numPrice.Value,
                customerName: txtCustomer.Text,
                out var error
            );

            if (!ok)
            {
                MessageBox.Show(error, "Sale blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateStockAndDefaults();
                return;
            }

            MessageBox.Show("Sale saved and stock updated!");
            ClearInputs();
            LoadBrands();
        }

        private void ClearInputs()
        {
            txtCustomer.Clear();
            numQuantity.Value = 1;
            // optional reset:
            numPrice.Value = 0;

            if (cmbBrand.Items.Count > 0)
                cmbBrand.SelectedIndex = 0;
        }


    }
}
