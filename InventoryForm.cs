using Bike_STore_Project;
using System;
using System.ComponentModel;
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
            Load += InventoryForm_Load;
            txtSearch.TextChanged += (s, a) => LoadData(txtSearch.Text);
            btnRefresh.Click += (s, a) => LoadData();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        private void InventoryForm_Load(object sender, EventArgs e)
        {
            LoadData();

            txtSearch.TextChanged += (s, a) => LoadData(txtSearch.Text);
            btnRefresh.Click += (s, a) => LoadData();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        private void SetupGrid()
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.ReadOnly = true;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.Columns.Clear();
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = "ID",
                Visible = false
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Serial",
                HeaderText = "Serial",
                Width = 150
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Model",
                HeaderText = "Model",
                Width = 250
            });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                HeaderText = "Price",
                Width = 100,
                DefaultCellStyle = { Format = "C2" }
            });

            dgvProducts.CellDoubleClick += (s, e) => EditSelected();
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
            if (dgvProducts.CurrentRow?.DataBoundItem is Product p) return p;
            return null;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var dlg = new ProductEditForm();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _repo.Insert(dlg.Product);
                LoadData(txtSearch.Text);
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e) => EditSelected();

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

            var dlg = new ProductEditForm(full);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                _repo.Update(dlg.Product);
                LoadData(txtSearch.Text);
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

            var confirm = MessageBox.Show(
                $"Delete {selected.Model} (Serial {selected.Serial})?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            _repo.Delete(selected.Id);
            LoadData(txtSearch.Text);
        }
    }
}
