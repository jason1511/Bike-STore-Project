using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
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

        public int QuantityReceived => (int)numQuantity.Value;
        public decimal UnitCost => numPrice.Value;
        public DateTime ReceivedAt => DateTime.Now;   
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
                numPrice.Value = lot.UnitCost;
            }
        }

        public ProductEditForm(ProductEditMode mode)
        {
            InitializeComponent();
            numQuantity.Minimum = 1;
            numPrice.Minimum = 0.01m;

            _mode = mode;

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
                _priceTouched = true;
            }


            Product = new Product();

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

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
                // lock identity, user inputs batch details
                numQuantity.Value = 1;
                numPrice.Value = 0;
            }
        }
        private void ApplyModeUi()
        {
            if (_mode == ProductEditMode.Identity)
            {
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
                Text = "Receive Stock (New Batch)";
                lblQuantity.Text = "Qty Received";
                lblPrice.Text = "Unit Cost";

                // Show batch fields
                numQuantity.Visible = true;
                numPrice.Visible = true;

                // IMPORTANT:
                // Allow identity input for NEW stock receiving
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


        public ProductEditForm(Product existing) : this(existing, ProductEditMode.Identity) { }



        private void BtnOk_Click(object? sender, EventArgs e)
        {
            // Validate identity always (both modes)
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
            if (numQuantity.Value <= 0)
            {
                MessageBox.Show("Quantity must be at least 1.");
                return;
            }
            if (numPrice.Value <= 0)
            {
                MessageBox.Show("Unit cost must be greater than 0.");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        

    }
}

