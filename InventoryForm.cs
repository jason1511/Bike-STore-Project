using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class InventoryForm : Form
    {
        private readonly IStoreBackend _backend = AppServices.Backend;
        private BindingList<WebsiteBike> _bikes = new();

        public InventoryForm()
        {
            InitializeComponent(); SetupGrid();
            btnAdd.Text = "ADD BIKE"; btnEdit.Text = "EDIT BIKE"; btnDelete.Text = "DEACTIVATE";
            btnReceiveStock.Text = "TAMBAH STOK"; btnReceiveStock.Visible = true; btnReceiveStock.Width = 150;
            Load += async (_, __) => { await LoadDataAsync(); ApplyPermissions(); };
            txtSearch.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) await LoadDataAsync(txtSearch.Text); };
            btnRefresh.Click += async (_, __) => await LoadDataAsync(txtSearch.Text);
            btnAdd.Click += async (_, __) => await AddBikeAsync(); btnEdit.Click += async (_, __) => await EditBikeAsync();
            btnReceiveStock.Click += async (_, __) => await ReceiveStockAsync(); btnDelete.Click += async (_, __) => await ToggleActiveAsync();
            dgvProducts.CellDoubleClick += async (_, __) => { if (Permissions.CanEditInventory) await EditBikeAsync(); };
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
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName="Price", Name="Price", HeaderText="Selling price", Width=120, DefaultCellStyle={Format="C0",FormatProvider=StoreFormat.Culture} });
            dgvProducts.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName="InStock", Name="InStock", HeaderText="Active", Width=70 });
        }

        private static DataGridViewTextBoxColumn Column(string property,string title,int width)
            => new() { DataPropertyName=property, Name=property, HeaderText=title, Width=width };

        private async Task LoadDataAsync(string? search=null)
        {
            try
            {
                UseWaitCursor = true;
                _bikes = new BindingList<WebsiteBike>((await _backend.GetBikesAsync(search)).ToList());
                dgvProducts.DataSource = _bikes; UiTheme.StyleGrid(dgvProducts);
            }
            catch(Exception ex){MessageBox.Show("Failed to load bike inventory: "+ex.Message,"Inventory",MessageBoxButtons.OK,MessageBoxIcon.Error);}
            finally { UseWaitCursor = false; }
        }

        private WebsiteBike? Selected(bool showMessage=true)
        {
            var bike=dgvProducts.CurrentRow?.DataBoundItem as WebsiteBike;
            if(bike==null&&showMessage)MessageBox.Show("Select a bicycle first."); return bike;
        }

        private async Task AddBikeAsync()
        {
            try
            {
                var brands = await _backend.GetBrandsAsync();
                using var dialog=new WebsiteBikeEditorDialog(null, brands); if(dialog.ShowDialog(this)!=DialogResult.OK)return;
                await _backend.SaveBikeAsync(dialog.Bike,true);await LoadDataAsync(txtSearch.Text);MessageBox.Show("Bicycle added with its colour stock.","Inventory");
            }
            catch(Exception ex){MessageBox.Show(ex.Message,"Bicycle not saved",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }

        private async Task EditBikeAsync()
        {
            var selected=Selected();if(selected==null)return;
            try
            {
                var fresh=await _backend.GetBikeAsync(selected.Id);if(fresh==null)return;
                var brands = await _backend.GetBrandsAsync();
                using var dialog=new WebsiteBikeEditorDialog(fresh, brands);if(dialog.ShowDialog(this)!=DialogResult.OK)return;
                await _backend.SaveBikeAsync(dialog.Bike,false);await LoadDataAsync(txtSearch.Text);MessageBox.Show("Bicycle and colour variants updated.","Inventory");
            }
            catch(Exception ex){MessageBox.Show(ex.Message,"Bicycle not updated",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }

        private async Task ReceiveStockAsync()
        {
            try
            {
                using var dialog=new WebsiteReceiveStockDialog(await _backend.GetBikesAsync(),Selected(false)?.Id,_backend.UsesFifoPurchaseCost);
                if(dialog.ShowDialog(this)!=DialogResult.OK)return;
                await _backend.ReceiveStockAsync(dialog.BikeId,dialog.ColorName,dialog.ColorHex,dialog.ColorImage,dialog.Quantity,dialog.UnitCost,dialog.ReceivedAt,dialog.Notes);
                await LoadDataAsync(txtSearch.Text);MessageBox.Show("Stock received and movement recorded.","Tambah Stok",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch(Exception ex){MessageBox.Show(ex.Message,"Stock not received",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }

        private async Task ToggleActiveAsync()
        {
            var bike=Selected();if(bike==null)return;var next=!bike.InStock;
            if(MessageBox.Show($"{(next?"Reactivate":"Deactivate")} {bike.Brand} {bike.Name}?", "Catalogue status",MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
            try{await _backend.SetBikeActiveAsync(bike.Id,next);await LoadDataAsync(txtSearch.Text);}catch(Exception ex){MessageBox.Show(ex.Message,"Status update failed");}
        }
    }
}
