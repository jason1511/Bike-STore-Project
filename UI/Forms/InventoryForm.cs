using System;
using System.ComponentModel;
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
            btnAdd.Text = Strings.Get("Inventory_Add"); btnEdit.Text = Strings.Get("Inventory_Edit"); btnDelete.Text = Strings.Get("Inventory_Deactivate");
            btnReceiveStock.Text = Strings.Get("Inventory_Receive"); btnReceiveStock.Visible = true; btnReceiveStock.Width = 145;
            Load += async (_, __) => { await LoadDataAsync(); ApplyPermissions(); };
            txtSearch.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) await LoadDataAsync(txtSearch.Text); };
            btnRefresh.Click += async (_, __) => await LoadDataAsync(txtSearch.Text);
            btnAdd.Click += async (_, __) => await AddBikeAsync(); btnEdit.Click += async (_, __) => await EditBikeAsync();
            btnReceiveStock.Click += async (_, __) => await ReceiveStockAsync(); btnDelete.Click += async (_, __) => await ToggleActiveAsync();
            dgvProducts.CellDoubleClick += async (_, __) => { if (Permissions.CanEditInventory) await EditBikeAsync(); };
            dgvProducts.SelectionChanged += (_, __) => UpdateSelectionActions();
        }

        private void ApplyPermissions()
        {
            btnAdd.Enabled = Permissions.CanReceiveInventory; btnEdit.Enabled = Permissions.CanEditInventory;
            btnReceiveStock.Enabled = Permissions.CanReceiveInventory; btnDelete.Enabled = Permissions.CanDeleteInventory;
            UpdateSelectionActions();
        }

        private void SetupGrid()
        {
            dgvProducts.AutoGenerateColumns = false; dgvProducts.Columns.Clear(); dgvProducts.ReadOnly = true;
            dgvProducts.MultiSelect = false; dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.AllowUserToAddRows = false; dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.Columns.Add(Column("Brand", Strings.Get("Inventory_Brand"), 130));
            dgvProducts.Columns.Add(Column("Name", Strings.Get("Inventory_Model"), 170));
            dgvProducts.Columns.Add(Column("ColorSummary", Strings.Get("Inventory_Colours"), 310));
            dgvProducts.Columns.Add(Column("StockQty", Strings.Get("Inventory_TotalStock"), 90));
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName="Price", Name="Price", HeaderText=Strings.Get("Inventory_SellingPrice"), Width=120, DefaultCellStyle={Format="C0",FormatProvider=StoreFormat.Culture} });
            dgvProducts.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName="InStock", Name="InStock", HeaderText=Strings.Get("Inventory_Active"), Width=70 });
        }

        private static DataGridViewTextBoxColumn Column(string property,string title,int width)
            => new() { DataPropertyName=property, Name=property, HeaderText=title, Width=width };

        private async Task LoadDataAsync(string? search=null)
        {
            try
            {
                UseWaitCursor = true;
                _bikes = new BindingList<WebsiteBike>((await _backend.GetBikesAsync(search)).ToList());
                dgvProducts.DataSource = _bikes; UiTheme.StyleGrid(dgvProducts); UpdateSelectionActions();
            }
            catch(Exception ex){MessageBox.Show(Strings.Format("Inventory_LoadFailed",ex.Message),Strings.Get("Inventory_Title"),MessageBoxButtons.OK,MessageBoxIcon.Error);}
            finally { UseWaitCursor = false; }
        }

        private WebsiteBike? Selected(bool showMessage=true)
        {
            var bike=dgvProducts.CurrentRow?.DataBoundItem as WebsiteBike;
            if(bike==null&&showMessage)MessageBox.Show(Strings.Get("Inventory_SelectBike")); return bike;
        }

        private async Task AddBikeAsync()
        {
            try
            {
                var brands = await _backend.GetBrandsAsync();
                using var dialog=new WebsiteBikeEditorDialog(null, brands); if(dialog.ShowDialog(this)!=DialogResult.OK)return;
                await _backend.SaveBikeAsync(dialog.Bike,true);await LoadDataAsync(txtSearch.Text);MessageBox.Show(Strings.Get("Inventory_Added"),Strings.Get("Inventory_Title"));
            }
            catch(Exception ex){MessageBox.Show(ex.Message,Strings.Get("Inventory_NotSaved"),MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }

        private async Task EditBikeAsync()
        {
            var selected=Selected();if(selected==null)return;
            try
            {
                var fresh=await _backend.GetBikeAsync(selected.Id);if(fresh==null)return;
                var brands = await _backend.GetBrandsAsync();
                using var dialog=new WebsiteBikeEditorDialog(fresh, brands);if(dialog.ShowDialog(this)!=DialogResult.OK)return;
                await _backend.SaveBikeAsync(dialog.Bike,false);await LoadDataAsync(txtSearch.Text);MessageBox.Show(Strings.Get("Inventory_Updated"),Strings.Get("Inventory_Title"));
            }
            catch(Exception ex){MessageBox.Show(ex.Message,Strings.Get("Inventory_NotUpdated"),MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }

        private async Task ReceiveStockAsync()
        {
            try
            {
                using var dialog=new WebsiteReceiveStockDialog(await _backend.GetBikesAsync(),Selected(false)?.Id,_backend.UsesFifoPurchaseCost);
                if(dialog.ShowDialog(this)!=DialogResult.OK)return;
                await _backend.ReceiveStockAsync(dialog.BikeId,dialog.ColorName,dialog.ColorHex,dialog.ColorImage,dialog.Quantity,dialog.UnitCost,dialog.ReceivedAt,dialog.Notes);
                await LoadDataAsync(txtSearch.Text);MessageBox.Show(Strings.Get("Inventory_StockReceived"),Strings.Get("Inventory_Receive"),MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch(Exception ex){MessageBox.Show(ex.Message,Strings.Get("Inventory_StockNotReceived"),MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }

        private async Task ToggleActiveAsync()
        {
            var bike=Selected();if(bike==null)return;var next=!bike.InStock;
            if(MessageBox.Show(Strings.Format("Inventory_StatusQuestion",Strings.Get(next?"Inventory_Reactivate":"Inventory_Deactivate"),bike.Brand,bike.Name), Strings.Get("Inventory_StatusTitle"),MessageBoxButtons.YesNo,MessageBoxIcon.Question)!=DialogResult.Yes)return;
            try{await _backend.SetBikeActiveAsync(bike.Id,next);await LoadDataAsync(txtSearch.Text);}catch(Exception ex){MessageBox.Show(ex.Message,Strings.Get("Inventory_StatusFailed"));}
        }

        private void UpdateSelectionActions()
        {
            var selected = Selected(false);
            btnEdit.Enabled = Permissions.CanEditInventory && selected != null;
            btnReceiveStock.Enabled = Permissions.CanReceiveInventory && selected != null;
            btnDelete.Enabled = Permissions.CanDeleteInventory && selected != null;
            btnDelete.Text = Strings.Get(selected?.InStock == false ? "Inventory_Reactivate" : "Inventory_Deactivate");
            UiTheme.StyleButton(btnDelete, selected?.InStock == true);
        }
    }
}
