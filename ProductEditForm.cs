using System;
using System.Linq;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public enum ProductEditMode
    {
        Identity,
        ReceiveStock
    }

    public partial class ProductEditForm : Form
    {
        private readonly ProductEditMode _mode;

        // Exposed values used by ReceiveStock workflow
        public int QuantityReceived => (int)numQuantity.Value;
        public decimal UnitCost => numPrice.Value;
        public DateTime ReceivedAt => DateTime.Now;

        // The identity being edited/created
        public Product Product { get; private set; }

        private readonly ProductRepository _repo = new();
        private bool _priceTouched = false;

        public ProductEditForm() : this(ProductEditMode.Identity)
        {
        }

        public ProductEditForm(StockLotRow lot, ProductEditMode mode) : this(mode)
        {
            // identity
            txtBrand.Text = lot.Brand;
            txtType.Text = lot.Type;
            txtColor.Text = lot.Color ?? "";

            if (mode == ProductEditMode.ReceiveStock)
            {
                // batch values
                numQuantity.Value = Math.Max(1, lot.QtyReceived);
                // Ensure we don't go below Minimum (will throw otherwise)
                numPrice.Value = ClampToMinMax(lot.UnitCost, numPrice);
            }
        }

        public ProductEditForm(ProductEditMode mode)
        {
            InitializeComponent();

            _mode = mode;

            // sensible defaults / validation bounds
            numQuantity.Minimum = 1;
            numPrice.Minimum = 0.01m;

            // these are also set in Designer, but keeping here doesn't hurt
            txtBrand.CharacterCasing = CharacterCasing.Upper;
            txtType.CharacterCasing = CharacterCasing.Upper;
            txtColor.CharacterCasing = CharacterCasing.Upper;

            SetupAutocomplete();

            if (_mode == ProductEditMode.Identity)
            {
                txtBrand.TextChanged += (s, e) =>
                {
                    SetupTypeAutocomplete();
                    SetupColorAutocomplete();
                };

                txtType.TextChanged += (s, e) =>
                {
                    SetupColorAutocomplete();
                };
            }
            else
            {
                // In ReceiveStock mode, cost is typically intentional
                _priceTouched = true;
            }

            Product = new Product();

            // Buttons
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            ApplyModeUi();
        }

        public ProductEditForm(Product existing, ProductEditMode mode) : this(mode)
        {
            Product = existing;

            txtBrand.Text = Product.Brand;
            txtType.Text = Product.Type;
            txtColor.Text = Product.Color ?? "";

            if (mode == ProductEditMode.Identity)
            {
                _priceTouched = true;
            }
            else // ReceiveStock
            {
                // lock identity? (you currently allow identity input; keeping your behaviour)
                // user inputs batch details
                numQuantity.Value = 1;
                numPrice.Value = numPrice.Minimum; // IMPORTANT: don't set below Minimum
            }
        }

        public ProductEditForm(Product existing) : this(existing, ProductEditMode.Identity)
        {
        }

        private void ApplyModeUi()
        {
            if (_mode == ProductEditMode.Identity)
            {
                lblQuantity.Visible = false;
                lblPrice.Visible = false;
                Text = "Edit Product";

                // Identity mode: allow editing identity
                txtBrand.Enabled = true;
                txtType.Enabled = true;
                txtColor.Enabled = true;

                // Hide batch fields
                numQuantity.Visible = false;
                numPrice.Visible = false;
            }
            else
            {
                lblQuantity.Visible = true;
                lblPrice.Visible = true;
                Text = "Receive Stock (New Batch)";
                lblQuantity.Text = "Qty Received";
                lblPrice.Text = "Unit Cost";

                // Show batch fields
                numQuantity.Visible = true;
                numPrice.Visible = true;

                // Allow identity input for NEW stock receiving (keeping your current intent)
                txtBrand.Enabled = true;
                txtType.Enabled = true;
                txtColor.Enabled = true;
            }
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
            SetupTypeAutocomplete();
            SetupColorAutocomplete();
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

            if (!string.IsNullOrWhiteSpace(txtBrand.Text) && !string.IsNullOrWhiteSpace(txtType.Text))
                src.AddRange(_repo.GetDistinctColors(txtBrand.Text, txtType.Text).ToArray());

            txtColor.AutoCompleteCustomSource = src;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            // Validate identity always (both modes)
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

            // Always store identity into Product (both modes)
            Product.Brand = txtBrand.Text.Trim().ToUpperInvariant();
            Product.Type = txtType.Text.Trim().ToUpperInvariant();
            Product.Color = string.IsNullOrWhiteSpace(txtColor.Text)
                ? null
                : txtColor.Text.Trim().ToUpperInvariant();

            if (_mode == ProductEditMode.Identity)
            {
                // Identity edit only (no stock fields)
                Product.Quantity = 0;
                Product.Price = 0;

                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            // ReceiveStock mode validation
            if (numQuantity.Value < numQuantity.Minimum)
            {
                MessageBox.Show("Quantity must be at least 1.");
                numQuantity.Focus();
                return;
            }

            if (numPrice.Value < numPrice.Minimum)
            {
                MessageBox.Show("Unit cost must be greater than 0.");
                numPrice.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private static decimal ClampToMinMax(decimal value, NumericUpDown nud)
        {
            if (value < nud.Minimum) return nud.Minimum;
            if (value > nud.Maximum) return nud.Maximum;
            return value;
        }
    }
}
