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
    public partial class ProductEditForm : Form
    {
        public Product Product { get; private set; }
        private readonly ProductRepository _repo = new();
        private bool _priceTouched = false;
        public ProductEditForm()
        {
            InitializeComponent();

            txtBrand.CharacterCasing = CharacterCasing.Upper;
            txtType.CharacterCasing = CharacterCasing.Upper;
            txtColor.CharacterCasing = CharacterCasing.Upper;
            SetupAutocomplete();
            txtBrand.TextChanged += (s, e) =>
            {
                SetupTypeAutocomplete();
                SetupColorAutocomplete();
                TryAutoFillPrice();
            };

            txtType.TextChanged += (s, e) =>
            {
                SetupColorAutocomplete();
                TryAutoFillPrice();
            };
            numPrice.ValueChanged += (s, e) => _priceTouched = true;
            txtColor.TextChanged += (s, e) => TryAutoFillPrice();
            Product = new Product();

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
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

        private void TryAutoFillPrice()
        {
            if (_priceTouched) return;

            if (string.IsNullOrWhiteSpace(txtBrand.Text) || string.IsNullOrWhiteSpace(txtType.Text))
                return;

            var price = _repo.GetExistingPrice(txtBrand.Text, txtType.Text, txtColor.Text);
            if (price.HasValue && price.Value >= 0)
                numPrice.Value = price.Value;
        }

        public ProductEditForm(Product existing) : this()
        {
            Product = existing;

            txtBrand.Text = Product.Brand;
            txtType.Text = Product.Type;
            txtColor.Text = Product.Color ?? "";
            numQuantity.Value = Product.Quantity;
            numPrice.Value = Product.Price;

            _priceTouched = true;
        }


        private void BtnOk_Click(object? sender, EventArgs e)
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

            Product.Brand = txtBrand.Text.Trim().ToUpperInvariant();
            Product.Type = txtType.Text.Trim().ToUpperInvariant();
            Product.Color = string.IsNullOrWhiteSpace(txtColor.Text) ? null : txtColor.Text.Trim().ToUpperInvariant();
            Product.Quantity = (int)numQuantity.Value;
            Product.Price = numPrice.Value;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

