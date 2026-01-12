using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;



namespace Bike_STore_Project
{
    public partial class InventoryForm : Form
    {
        private readonly ProductRepository _repo = new();
        private BindingList<StockLotRow> _bindingList = new();



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
            // btnReceiveStock.Click += (s, e) => ReceiveStockForSelected();
            btnReceiveStock.Visible = false;   // or Enabled = false;


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
                DataPropertyName = "LotId",
                HeaderText = "Lot ID",
                Visible = false
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductId",
                HeaderText = "Product ID",
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
                DataPropertyName = "QtyReceived",
                HeaderText = "Received",
                Width = 80
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "QtyRemaining",
                HeaderText = "Remaining",
                Width = 90
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UnitCost",
                HeaderText = "Unit Cost",
                Width = 100,
                DefaultCellStyle = { Format = "C2" }
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ReceivedAt",
                HeaderText = "Received At",
                Width = 140,
                DefaultCellStyle = { Format = "yyyy-MM-dd HH:mm" }
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Notes",
                HeaderText = "Notes",
                Width = 220
            });



        }

        private void LoadData(string? filter = null)
        {
            try
            {
                var list = _repo.GetStockLots(filter);
                _bindingList = new BindingList<StockLotRow>(list);
                dgvProducts.DataSource = _bindingList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load products: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        private StockLotRow? GetSelected()
        {
            return dgvProducts.CurrentRow?.DataBoundItem as StockLotRow;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            // Use ReceiveStock mode for Add (receive a batch)
            using var dlg = new ProductEditForm(ProductEditMode.ReceiveStock);

            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                // Create or reuse the identity row (brand/type/color)
                var productId = _repo.GetOrCreateProductId(
                    dlg.Product.Brand,
                    dlg.Product.Type,
                    dlg.Product.Color
                );

                // Insert the batch into stock_lots
                _repo.ReceiveBatch(
                    productId: productId,
                    qtyReceived: dlg.QuantityReceived,
                    unitCost: dlg.UnitCost,
                    receivedAt: DateTime.Now,
                    notes: null
                );

                LoadData(txtSearch.Text);
                MessageBox.Show("Stock batch added!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add stock failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void EditSelected()
        {
            var selected = GetSelected();
            if (selected == null)
            {
                MessageBox.Show("Select a batch first.");
                return;
            }

            using var dlg = new ProductEditForm(selected, ProductEditMode.ReceiveStock);
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                _repo.UpdateStockLot(
                    lotId: selected.LotId,
                    newQtyReceived: dlg.QuantityReceived,
                    newUnitCost: dlg.UnitCost
                );

                LoadData(txtSearch.Text);
                MessageBox.Show("Batch updated!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update batch failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null)
            {
                MessageBox.Show("Select a Batch first.");
                return;
            }
            if (selected.QtyRemaining != selected.QtyReceived)
            {
                MessageBox.Show("Can't delete this batch because some items were already sold from it.");
                return;
            }
            var label = $"{selected.Brand} {selected.Type} - {selected.ReceivedAt:yyyy-MM-dd HH:mm}";
            var confirm = MessageBox.Show(
                $"Delete batch: {label}?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            


            if (confirm != DialogResult.Yes) return;

            try
            {
                _repo.DeleteStockLot(selected.LotId);

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

