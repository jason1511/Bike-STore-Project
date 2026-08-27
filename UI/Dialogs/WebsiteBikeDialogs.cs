using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    internal sealed class WebsiteBikeEditorDialog : Form
    {
        private readonly TextBox _id=Box(),_name=Box(),_battery=Box(),_motor=Box(),_speed=Box(),_range=Box(),_weight=Box(),_safety=Box(),_image=Box(),_description=new(){Width=300,Height=64,Multiline=true};
        private readonly ComboBox _brand=new(){Width=300,DropDownStyle=ComboBoxStyle.DropDownList};
        private readonly NumericUpDown _price=new(){Width=180,Maximum=1_000_000_000,ThousandsSeparator=true};
        private readonly CheckBox _featured=new(){Text="Featured"},_active=new(){Text="Active",Checked=true};
        private readonly DataGridView _colors=new(){Dock=DockStyle.Fill,AllowUserToAddRows=false,RowHeadersVisible=false,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
        private readonly bool _editing;
        public WebsiteBike Bike { get; private set; }

        public WebsiteBikeEditorDialog(WebsiteBike? bike=null, IReadOnlyList<StoreBrand>? brands=null)
        {
            _editing=bike!=null;Bike=bike??new WebsiteBike();Text=_editing?$"Edit {bike!.Brand} {bike.Name}":"Tambah Sepeda";
            StartPosition=FormStartPosition.CenterParent;ClientSize=new Size(900,680);MinimumSize=new Size(820,600);
            _active.Enabled=AppSession.IsAdmin||!_editing;
            BuildColors();LoadBrands(brands ?? Array.Empty<StoreBrand>());BuildLayout();if(_editing)Fill(Bike);UiTheme.Apply(this);
        }

        private void BuildLayout()
        {
            var tabs=new TabControl{Dock=DockStyle.Fill};var details=new TabPage("Bike details");var variants=new TabPage("Colour variants & stock");
            var table=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=4,Padding=new Padding(18),AutoScroll=true};
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,120));table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,120));table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));
            Add(table,"Bike ID",_id,0);Add(table,"Brand *",_brand,0);Add(table,"Model *",_name,0);Add(table,"Selling price",_price,0);
            Add(table,"Battery",_battery,2);Add(table,"Motor",_motor,2);Add(table,"Top speed",_speed,2);Add(table,"Range",_range,2);Add(table,"Max weight",_weight,2);Add(table,"Safety",_safety,2);Add(table,"Main image",_image,2);Add(table,"Description",_description,2);
            var flags=new FlowLayoutPanel{AutoSize=true};flags.Controls.Add(_featured);flags.Controls.Add(_active);Add(table,"Visibility",flags,2);details.Controls.Add(table);
            var variantRoot=new TableLayoutPanel{Dock=DockStyle.Fill,RowCount=3,Padding=new Padding(16)};variantRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));variantRoot.RowStyles.Add(new RowStyle(SizeType.Percent,100));variantRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            variantRoot.Controls.Add(new Label{Text="Like the website editor: each colour has its own image and stock quantity. Changing a quantity creates a recorded stock movement.",AutoSize=true,Tag="muted"},0,0);variantRoot.Controls.Add(_colors,0,1);
            var colorActions=new FlowLayoutPanel{Dock=DockStyle.Fill,AutoSize=true};var add=new Button{Text="+ Add colour",Width=120};var remove=new Button{Text="Remove selected",Width=135};add.Click+=(_,__)=>AddColor();remove.Click+=(_,__)=>RemoveColor();colorActions.Controls.Add(add);colorActions.Controls.Add(remove);variantRoot.Controls.Add(colorActions,0,2);variants.Controls.Add(variantRoot);
            tabs.TabPages.Add(details);tabs.TabPages.Add(variants);
            var actions=new FlowLayoutPanel{Dock=DockStyle.Bottom,Height=58,FlowDirection=FlowDirection.RightToLeft,Padding=new Padding(12)};var save=new Button{Text="Save bicycle",Width=130,Height=34};var cancel=new Button{Text="Cancel",Width=90,Height=34,DialogResult=DialogResult.Cancel};save.Click+=(_,__)=>Save();actions.Controls.Add(save);actions.Controls.Add(cancel);Controls.Add(tabs);Controls.Add(actions);CancelButton=cancel;
        }

        private void BuildColors()
        {
            _colors.Columns.Add(new DataGridViewTextBoxColumn{Name="Name",HeaderText="Colour name"});_colors.Columns.Add(new DataGridViewTextBoxColumn{Name="Hex",HeaderText="Hex code"});
            _colors.Columns.Add(new DataGridViewTextBoxColumn{Name="Image",HeaderText="Image path"});_colors.Columns.Add(new DataGridViewTextBoxColumn{Name="Stock",HeaderText="Stock quantity",ValueType=typeof(int)});
        }
        private void AddColor(WebsiteBikeColor? c=null)=>_colors.Rows.Add(c?.Name??"",c?.Hex??"#cccccc",c?.Image??"",c?.StockQty??0);
        private void RemoveColor(){if(_colors.CurrentRow!=null)_colors.Rows.Remove(_colors.CurrentRow);}
        private void LoadBrands(IReadOnlyList<StoreBrand> brands){foreach(var brand in brands.Where(x=>x.IsActive))_brand.Items.Add(new BrandChoice(brand.Id,brand.Name));if(_brand.Items.Count>0)_brand.SelectedIndex=0;}
        private void Fill(WebsiteBike b){_id.Text=b.Id;_id.ReadOnly=true;SelectBrand(b.BrandId,b.Brand);_name.Text=b.Name;_battery.Text=b.Battery;_motor.Text=b.Motor;_speed.Text=b.TopSpeed;_range.Text=b.Range;_weight.Text=b.MaxWeight;_safety.Text=b.Safety;_image.Text=b.Image;_description.Text=b.Description;_price.Value=Math.Min(_price.Maximum,b.Price);_featured.Checked=b.Featured;_active.Checked=b.InStock;foreach(var c in b.Colors)AddColor(c);}
        private void SelectBrand(string id,string name){for(var i=0;i<_brand.Items.Count;i++)if(_brand.Items[i] is BrandChoice b&&(b.Id==id||b.Name.Equals(name,StringComparison.OrdinalIgnoreCase))){_brand.SelectedIndex=i;return;}}
        private void Save()
        {
            if(_brand.SelectedItem is not BrandChoice brand){MessageBox.Show("Select a brand.");return;}if(string.IsNullOrWhiteSpace(_name.Text)){MessageBox.Show("Model name is required.");return;}
            var colors=new List<WebsiteBikeColor>();foreach(DataGridViewRow row in _colors.Rows){var name=Convert.ToString(row.Cells["Name"].Value)?.Trim()??"";if(string.IsNullOrWhiteSpace(name))continue;if(!int.TryParse(Convert.ToString(row.Cells["Stock"].Value),out var stock)||stock<0){MessageBox.Show($"Invalid stock for {name}.");return;}colors.Add(new WebsiteBikeColor{Name=name,Hex=Convert.ToString(row.Cells["Hex"].Value)??"#cccccc",Image=Convert.ToString(row.Cells["Image"].Value)??"",StockQty=stock});}
            if(colors.Count==0){MessageBox.Show("Add at least one colour.");return;}Bike.Id=_editing?Bike.Id:_id.Text.Trim();Bike.BrandId=brand.Id;Bike.Brand=brand.Name;Bike.Name=_name.Text.Trim();Bike.Battery=_battery.Text.Trim();Bike.Motor=_motor.Text.Trim();Bike.TopSpeed=_speed.Text.Trim();Bike.Range=_range.Text.Trim();Bike.MaxWeight=_weight.Text.Trim();Bike.Safety=_safety.Text.Trim();Bike.Image=_image.Text.Trim();Bike.Description=_description.Text.Trim();Bike.Price=_price.Value;Bike.Featured=_featured.Checked;Bike.InStock=_active.Checked;Bike.Colors=colors;Bike.ColorName=colors[0].Name;DialogResult=DialogResult.OK;Close();
        }
        private static TextBox Box()=>new(){Width=300};
        private static void Add(TableLayoutPanel table,string label,Control control,int startColumn){var row=table.RowCount++;table.RowStyles.Add(new RowStyle(SizeType.AutoSize));table.Controls.Add(new Label{Text=label,AutoSize=true,Padding=new Padding(0,7,0,0)},startColumn,row);table.Controls.Add(control,startColumn+1,row);}
        private sealed record BrandChoice(string Id,string Name){public override string ToString()=>Name;}
    }

    internal sealed class WebsiteReceiveStockDialog : Form
    {
        private readonly ComboBox _bike=new(){Width=330,DropDownStyle=ComboBoxStyle.DropDownList},_color=new(){Width=220,DropDownStyle=ComboBoxStyle.DropDownList};
        private readonly TextBox _newColor=new(){Width=220},_hex=new(){Width=120,Text="#cccccc"},_image=new(){Width=280},_notes=new(){Width=280,Height=55,Multiline=true};
        private readonly NumericUpDown _quantity=new(){Width=140,Minimum=1,Maximum=1_000_000,Value=1},_cost=new(){Width=180,Minimum=.01m,Maximum=1_000_000_000,ThousandsSeparator=true};
        private readonly DateTimePicker _received=new(){Width=190,Format=DateTimePickerFormat.Custom,CustomFormat="dd MMM yyyy HH:mm"};
        private readonly Label _summary=new(){AutoSize=true,Tag="muted"};
        private readonly bool _usesPurchaseCost;
        public string BikeId= "",ColorName="",ColorHex="",ColorImage="",Notes="";public int Quantity;public decimal UnitCost;public DateTime ReceivedAt;
        public WebsiteReceiveStockDialog(IReadOnlyList<WebsiteBike> bikes,string? selectedId=null,bool usesPurchaseCost=true)
        {
            _usesPurchaseCost=usesPurchaseCost;
            Text="Tambah Stok";StartPosition=FormStartPosition.CenterParent;ClientSize=new Size(610,550);FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;
            foreach(var b in bikes)_bike.Items.Add(new BikeChoice(b));if(_bike.Items.Count>0)_bike.SelectedIndex=0;if(!string.IsNullOrWhiteSpace(selectedId))for(var i=0;i<_bike.Items.Count;i++)if(_bike.Items[i] is BikeChoice x&&x.Bike.Id==selectedId)_bike.SelectedIndex=i;
            Build();_bike.SelectedIndexChanged+=(_,__)=>LoadColors();_color.SelectedIndexChanged+=(_,__)=>UpdateNewColourFields();_quantity.ValueChanged+=(_,__)=>UpdateSummary();LoadColors();UiTheme.Apply(this);
        }
        private void Build(){var table=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,Padding=new Padding(24),AutoScroll=true};table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,175));table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));Add(table,"Bicycle",_bike);Add(table,"Existing/new colour",_color);Add(table,"New colour name",_newColor);Add(table,"Colour hex",_hex);Add(table,"Colour image path",_image);Add(table,"Quantity received",_quantity);if(_usesPurchaseCost)Add(table,"Unit purchase cost",_cost);else Add(table,"Online stock",new Label{Text="Quantity is recorded by the Cloudflare store workflow.",AutoSize=true,Tag="muted"});Add(table,"Received at",_received);Add(table,"Notes / reference",_notes);Add(table,"Stock after receipt",_summary);var actions=new FlowLayoutPanel{AutoSize=true,FlowDirection=FlowDirection.RightToLeft};var save=new Button{Text="Receive stock",Width=125,Height=34};var cancel=new Button{Text="Cancel",Width=90,Height=34,DialogResult=DialogResult.Cancel};save.Click+=(_,__)=>Save();actions.Controls.Add(save);actions.Controls.Add(cancel);Add(table,"",actions);Controls.Add(table);CancelButton=cancel;}
        private void LoadColors(){_color.Items.Clear();if(_bike.SelectedItem is BikeChoice choice){foreach(var c in choice.Bike.Colors)_color.Items.Add(new ColorChoice(c));}_color.Items.Add("+ Add new colour");_color.SelectedIndex=0;UpdateNewColourFields();}
        private void UpdateNewColourFields(){var isNew=_color.SelectedItem is string;_newColor.Enabled=isNew;_hex.Enabled=isNew;_image.Enabled=isNew;if(!isNew&&_color.SelectedItem is ColorChoice c){_hex.Text=c.Color.Hex;_image.Text=c.Color.Image;}UpdateSummary();}
        private void UpdateSummary(){var current=_color.SelectedItem is ColorChoice c?c.Color.StockQty:0;_summary.Text=$"{current} + {(int)_quantity.Value} = {current+(int)_quantity.Value} units";}
        private void Save(){if(_bike.SelectedItem is not BikeChoice bike)return;var isNew=_color.SelectedItem is string;var name=isNew?_newColor.Text.Trim():((_color.SelectedItem as ColorChoice)?.Color.Name??"");if(string.IsNullOrWhiteSpace(name)){MessageBox.Show("Enter the new colour name.");return;}if(_usesPurchaseCost&&_cost.Value<=0){MessageBox.Show("Enter the unit purchase cost.");return;}BikeId=bike.Bike.Id;ColorName=name;ColorHex=_hex.Text;ColorImage=_image.Text.Trim();Quantity=(int)_quantity.Value;UnitCost=_cost.Value;ReceivedAt=_received.Value;Notes=_notes.Text.Trim();DialogResult=DialogResult.OK;Close();}
        private static void Add(TableLayoutPanel table,string label,Control control){var row=table.RowCount++;table.RowStyles.Add(new RowStyle(SizeType.AutoSize));table.Controls.Add(new Label{Text=label,AutoSize=true,Padding=new Padding(0,7,0,0)},0,row);table.Controls.Add(control,1,row);}
        private sealed record BikeChoice(WebsiteBike Bike){public override string ToString()=>$"{Bike.Brand} {Bike.Name}";}
        private sealed record ColorChoice(WebsiteBikeColor Color){public override string ToString()=>$"{Color.Name} — stock {Color.StockQty}";}
    }
}
