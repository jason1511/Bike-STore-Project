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
    public partial class InventoryForm : Form
    {
        private readonly ProductRepository _repo = new();
        private BindingList<Product> _bindingList = new();

        public InventoryForm()
        {
            InitializeComponent();
            SetupGrid();

            // wire events ONCE
            Load += InventoryForm_Load;
            txtSearch.TextChanged += (s, e) => LoadData(txtSearch.Text);
            btnRefresh.Click += (s, e) => LoadData();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += (s, e) => EditSelected();
            btnDelete.Click += BtnDelete_Click;

            dgvProducts.CellDoubleClick += (s, e) => EditSelected();
        }

        private void InventoryForm_Load(object? sender, EventArgs e)
        {
            LoadData();
        }

        private void SetupGrid()
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.ReadOnly = true;

            dgvProducts.Columns.Clear();

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = "ID",
                Visible = false
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Brand",
                HeaderText = "Brand",
                Width = 140
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Type",
                HeaderText = "Type",
                Width = 180
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Color",
                HeaderText = "Color",
                Width = 120
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity",
                HeaderText = "Qty",
                Width = 70
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                HeaderText = "Price",
                Width = 90,
                DefaultCellStyle = { Format = "C2" }
            });
        }

        private void LoadData(string? filter = null)
        {
            try
            {
                var list = _repo.GetAll(filter);
                _bindingList = new BindingList<Product>(list);
                dgvProducts.DataSource = _bindingList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load products: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Product? GetSelected()
        {
            return dgvProducts.CurrentRow?.DataBoundItem as Product;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var dlg = new ProductEditForm();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _repo.Insert(dlg.Product);
                    LoadData(txtSearch.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Add failed: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EditSelected()
        {
            var selected = GetSelected();
            if (selected == null)
            {
                MessageBox.Show("Select a product first.");
                return;
            }

            var full = _repo.GetById(selected.Id);
            if (full == null)
            {
                MessageBox.Show("Product not found.");
                LoadData();
                return;
            }

            using var dlg = new ProductEditForm(full);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _repo.Update(dlg.Product);
                    LoadData(txtSearch.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Update failed: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null)
            {
                MessageBox.Show("Select a product first.");
                return;
            }

            var label = $"{selected.Brand} {selected.Type}";
            var confirm = MessageBox.Show(
                $"Delete {label}?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _repo.Delete(selected.Id);
                LoadData(txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

