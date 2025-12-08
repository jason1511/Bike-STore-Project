using Bike_STore_Project;
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
            Load += ProductEditForm_Load;
        }

        public ProductEditForm(Product existing) : this()
        {
            Product = existing;
        }

        private void ProductEditForm_Load(object? sender, EventArgs e)
        {
            txtSerial.Text = Product.Serial;
            txtModel.Text = Product.Model;
            numPrice.Value = Product.Price;

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, a) => DialogResult = DialogResult.Cancel;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSerial.Text))
            {
                MessageBox.Show("Serial number is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                MessageBox.Show("Model is required.");
                return;
            }

            if (numPrice.Value < 0m)
            {
                MessageBox.Show("Price cannot be negative.");
                return;
            }

            Product.Serial = txtSerial.Text.Trim();
            Product.Model = txtModel.Text.Trim();
            Product.Price = numPrice.Value;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
