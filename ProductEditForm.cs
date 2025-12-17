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

        public ProductEditForm()
        {
            InitializeComponent();
            Product = new Product();

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }

        public ProductEditForm(Product existing) : this()
        {
            Product = existing;

            txtBrand.Text = Product.Brand;
            txtType.Text = Product.Type;
            txtColor.Text = Product.Color ?? "";
            numQuantity.Value = Product.Quantity;
            numPrice.Value = Product.Price;
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

            Product.Brand = txtBrand.Text.Trim();
            Product.Type = txtType.Text.Trim();
            Product.Color = string.IsNullOrWhiteSpace(txtColor.Text) ? null : txtColor.Text.Trim();
            Product.Quantity = (int)numQuantity.Value;
            Product.Price = numPrice.Value;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

