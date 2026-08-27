using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class InventoryForm : Form
    {
        private readonly WebsiteBikeRepository _repo = new();
        private BindingList<WebsiteBike> _bikes = new();

        public InventoryForm()
        {
            InitializeComponent(); SetupGrid();
            btnAdd.Text = "ADD BIKE"; btnEdit.Text = "EDIT BIKE"; btnDelete.Text = "DEACTIVATE";
            btnReceiveStock.Text = "TAMBAH STOK"; btnReceiveStock.Visible = true; btnReceiveStock.Width = 150;
            Load += (_, __) => { LoadData(); ApplyPermissions(); };
            txtSearch.TextChanged += (_, __) => LoadData(txtSearch.Text);
            btnRefresh.Click += (_, __) => LoadData(txtSearch.Text);
            btnAdd.Click += (_, __) => AddBike(); btnEdit.Click += (_, __) => EditBike();
            btnReceiveStock.Click += (_, __) => ReceiveStock(); btnDelete.Click += (_, __) => ToggleActive();
            dgvProducts.CellDoubleClick += (_, __) => { if (Permissions.CanEditInventory) EditBike(); };
        }

        private void ApplyPermissions()
        {
            btnAdd.Enabled = Permissions.CanReceiveInventory; btnEdit.Enabled = Permissions.CanEditInventory;
            btnReceiveStock.Enabled = Permissions.CanReceiveInventory; btnDelete.Enabled = Permissions.CanDeleteInventory;
        }

        private void SetupGrid()
        {
            dgvProducts.AutoGenerateColumns = false; dgvProducts.Columns.Clear(); dgvProducts.ReadOnly = true;
            dgvProducts.MultiSelect = false; dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.AllowUserToAddRows = false; dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.Columns.Add(Column("Brand", "Brand", 130));
            dgvProducts.Columns.Add(Column("Name", "Model", 170));
            dgvProducts.Columns.Add(Column("ColorSummary", "Colour variants", 310));
            dgvProducts.Columns.Add(Column("StockQty", "Total stock", 90));
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName="Price", Name="Price", HeaderText="Selling price", Width=120, DefaultCellStyle={Format="C0",FormatProvider=CultureInfo.GetCultureInfo("id-ID")} });
            dgvProducts.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName="InStock", Name="InStock", HeaderText="Active", Width=70 });
        }

        private static DataGridViewTextBoxColumn Column(string property,string title,int width)
            => new() { DataPropertyName=property, Name=property, HeaderText=title, Width=width };

        private void LoadData(string? search=null)
        {
            try { _bikes=new BindingList<WebsiteBike>(_repo.GetAll(search)); dgvProducts.DataSource=_bikes; UiTheme.StyleGrid(dgvProducts); }
            catch(Exception ex){MessageBox.Show("Failed to load bike inventory: "+ex.Message,"Inventory",MessageBoxButtons.OK,MessageBoxIcon.Error);}
        }

        private WebsiteBike? Selected(bool showMessage=true)
        {
            var bike=dgvProducts.CurrentRow?.DataBoundItem as WebsiteBike;
            if(bike==null&&showMessage)MessageBox.Show("Select a bicycle first."); return bike;
        }

        private void AddBike()
        {
            using var dialog=new WebsiteBikeEditorDialog(); if(dialog.ShowDialog(this)!=DialogResult.OK)return;
            try{_repo.SaveBike(dialog.Bike,true);LoadData(txtSearch.Text);MessageBox.Show("Bicycle added with its colour stock.","Inventory");}
            catch(Exception ex){MessageBox.Show(ex.Message,"Bicycle not saved",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }

        private void EditBike()
        {
            var selected=Selected();if(selected==null)return;
            var fresh=_repo.GetById(selected.Id);if(fresh==null)return;
            using var dialog=new WebsiteBikeEditorDialog(fresh);if(dialog.ShowDialog(this)!=DialogResult.OK)return;
            try{_repo.SaveBike(dialog.Bike,false);LoadData(txtSearch.Text);MessageBox.Show("Bicycle and colour variants updated.","Inventory");}
            catch(Exception ex){MessageBox.Show(ex.Message,"Bicycle not updated",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }

        private void ReceiveStock()
        {
            using var dialog=new WebsiteReceiveStockDialog(_repo.GetAll(),Selected(false)?.Id);
            if(dialog.ShowDialog(this)!=DialogResult.OK)return;
            try
            {
                _repo.ReceiveStock(dialog.BikeId,dialog.ColorName,dialog.ColorHex,dialog.ColorImage,dialog.Quantity,dialog.UnitCost,dialog.ReceivedAt,dialog.Notes);
                LoadData(txtSearch.Text);MessageBox.Show("Stock received and movement recorded.","Tambah Stok",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch(Exception ex){MessageBox.Show(ex.Message,"Stock not received",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }

        private void ToggleActive()
        {
            var bike=Selected();if(bike==null)return;var next=!bike.InStock;
            if(MessageBox.Show($"{(next?"Reactivate":"Deactivate")} {bike.Brand} {bike.Name}?", "Catalogue status",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
            try{_repo.SetActive(bike.Id,next);LoadData(txtSearch.Text);}catch(Exception ex){MessageBox.Show(ex.Message,"Status update failed");}
        }
    }
}
