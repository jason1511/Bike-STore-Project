using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public sealed class CatalogueManagementForm : Form
    {
        private readonly DataGridView _grid = new()
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
            MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells, RowHeadersVisible = false
        };
        private readonly TextBox _search = new() { Width = 240 };

        public CatalogueManagementForm()
        {
            Text = $"Bike Store - Catalogue - {AppSession.Username} ({AppSession.Role})";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 650);
            var menu = new MainMenuControl { Dock = DockStyle.Top };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, Padding = new Padding(12) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            actions.Controls.Add(new Label { Text = "Search", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }); actions.Controls.Add(_search);
            var add = new Button { Text = "Add bicycle", Enabled = Permissions.CanEditInventory }; add.Click += (_, __) => AddProduct();
            var edit = new Button { Text = "Edit selected", Enabled = Permissions.CanEditInventory }; edit.Click += (_, __) => EditProduct();
            var delete = new Button { Text = "Delete selected", Enabled = Permissions.CanDeleteInventory }; delete.Click += (_, __) => DeleteProduct();
            actions.Controls.AddRange(new Control[] { add, edit, delete });
            root.Controls.Add(actions, 0, 0); root.Controls.Add(_grid, 0, 1);
            Controls.Add(root); Controls.Add(menu);
            _search.TextChanged += (_, __) => LoadData();
            _grid.CellDoubleClick += (_, __) => { if (Permissions.CanEditInventory) EditProduct(); };
            Load += (_, __) => LoadData();
        }

        private void LoadData()
        {
            using var conn = Database.OpenConnection(); using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT p.id,p.brand,p.type,COALESCE(p.color,'') AS color,COALESCE(p.sell_price,0) AS sell_price,
 COALESCE(SUM(l.qty_remaining),0) AS stock,COALESCE(p.battery,'') AS battery,COALESCE(p.motor,'') AS motor,
 COALESCE(p.top_speed,'') AS top_speed,COALESCE(p.range_text,'') AS range_text,
 COALESCE(p.max_weight,'') AS max_weight,COALESCE(p.safety,'') AS safety,
 COALESCE(p.image_path,'') AS image_path,COALESCE(p.description,'') AS description,
 p.featured,p.is_active
FROM products p LEFT JOIN stock_lots l ON l.product_id=p.id
WHERE $q='' OR UPPER(p.brand) LIKE $like OR UPPER(p.type) LIKE $like OR UPPER(COALESCE(p.color,'')) LIKE $like
GROUP BY p.id ORDER BY p.brand,p.type,p.color;";
            var q = _search.Text.Trim().ToUpperInvariant(); cmd.Parameters.AddWithValue("$q", q); cmd.Parameters.AddWithValue("$like", $"%{q}%");
            using var reader = cmd.ExecuteReader(); var table = new DataTable(); table.Load(reader); _grid.DataSource = table;
            if (_grid.Columns.Contains("id")) _grid.Columns["id"].Visible = false;
            if (_grid.Columns.Contains("sell_price")) _grid.Columns["sell_price"].DefaultCellStyle.Format = "N0";
        }

        private void AddProduct()
        {
            using var dialog = new CatalogueEditDialog();
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                long id;
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx; cmd.CommandText = @"
INSERT INTO products
(brand,type,color,quantity,price,battery,motor,top_speed,range_text,max_weight,safety,image_path,
 description,featured,is_active,sell_price)
VALUES ($brand,$type,$color,0,0,$battery,$motor,$speed,$range,$weight,$safety,$image,$description,$featured,$active,$sell);
SELECT last_insert_rowid();";
                    Bind(cmd, dialog); id = Convert.ToInt64(cmd.ExecuteScalar() ?? 0L);
                }
                using (var brand = conn.CreateCommand())
                {
                    brand.Transaction = tx; brand.CommandText = "INSERT OR IGNORE INTO brands(name) VALUES ($name);";
                    brand.Parameters.AddWithValue("$name", dialog.Brand); brand.ExecuteNonQuery();
                }
                LocalAdminRepository.WriteAudit(conn, tx, "CREATE_PRODUCT", "products", id, $"{dialog.Brand} {dialog.Type} {dialog.Color}");
                tx.Commit(); LoadData();
            }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show(ex.Message, "Add bicycle failed"); }
        }

        private void EditProduct()
        {
            if (!Selected(out var row)) return;
            using var dialog = new CatalogueEditDialog(row);
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            var id = Convert.ToInt32(row["id"]);
            using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx; cmd.CommandText = @"
UPDATE products SET brand=$brand,type=$type,color=$color,battery=$battery,motor=$motor,
 top_speed=$speed,range_text=$range,max_weight=$weight,safety=$safety,image_path=$image,
 description=$description,featured=$featured,is_active=$active,sell_price=$sell WHERE id=$id;";
                    Bind(cmd, dialog); cmd.Parameters.AddWithValue("$id", id); cmd.ExecuteNonQuery();
                }
                using (var brand = conn.CreateCommand())
                {
                    brand.Transaction = tx; brand.CommandText = "INSERT OR IGNORE INTO brands(name) VALUES ($name);";
                    brand.Parameters.AddWithValue("$name", dialog.Brand); brand.ExecuteNonQuery();
                }
                LocalAdminRepository.WriteAudit(conn, tx, "UPDATE_PRODUCT", "products", id, $"{dialog.Brand} {dialog.Type} {dialog.Color}");
                tx.Commit(); LoadData();
            }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show(ex.Message, "Update bicycle failed"); }
        }

        private void DeleteProduct()
        {
            if (!Permissions.CanDeleteInventory || !Selected(out var row)) return;
            var id = Convert.ToInt32(row["id"]); var label = $"{row["brand"]} {row["type"]} {row["color"]}";
            using var conn = Database.OpenConnection();
            using (var check = conn.CreateCommand())
            {
                check.CommandText = "SELECT (SELECT COUNT(*) FROM stock_lots WHERE product_id=$id)+(SELECT COUNT(*) FROM invoice_items WHERE product_id=$id);";
                check.Parameters.AddWithValue("$id", id);
                if (Convert.ToInt32(check.ExecuteScalar() ?? 0) > 0) { MessageBox.Show("This bicycle has stock or transaction history and cannot be deleted. Deactivate it instead."); return; }
            }
            if (MessageBox.Show($"Delete {label}?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            using var tx = conn.BeginTransaction();
            try
            {
                using var cmd = conn.CreateCommand(); cmd.Transaction = tx; cmd.CommandText = "DELETE FROM products WHERE id=$id;"; cmd.Parameters.AddWithValue("$id", id); cmd.ExecuteNonQuery();
                LocalAdminRepository.WriteAudit(conn, tx, "DELETE_PRODUCT", "products", id, label); tx.Commit(); LoadData();
            }
            catch (Exception ex) { try { tx.Rollback(); } catch { } MessageBox.Show(ex.Message); }
        }

        private bool Selected(out DataRowView row)
        {
            row = null!;
            if (_grid.CurrentRow?.DataBoundItem is not DataRowView selected) { MessageBox.Show("Select a bicycle first."); return false; }
            row = selected; return true;
        }

        private static void Bind(SqliteCommand cmd, CatalogueEditDialog d)
        {
            cmd.Parameters.AddWithValue("$brand", d.Brand); cmd.Parameters.AddWithValue("$type", d.Type);
            cmd.Parameters.AddWithValue("$color", Db(d.Color)); cmd.Parameters.AddWithValue("$battery", Db(d.Battery));
            cmd.Parameters.AddWithValue("$motor", Db(d.Motor)); cmd.Parameters.AddWithValue("$speed", Db(d.TopSpeed));
            cmd.Parameters.AddWithValue("$range", Db(d.Range)); cmd.Parameters.AddWithValue("$weight", Db(d.MaxWeight));
            cmd.Parameters.AddWithValue("$safety", Db(d.Safety)); cmd.Parameters.AddWithValue("$image", Db(d.ImagePath));
            cmd.Parameters.AddWithValue("$description", Db(d.Description)); cmd.Parameters.AddWithValue("$featured", d.Featured ? 1 : 0);
            cmd.Parameters.AddWithValue("$active", d.Active ? 1 : 0); cmd.Parameters.AddWithValue("$sell", (double)d.SellPrice);
        }
        private static object Db(string value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
    }

    internal sealed class CatalogueEditDialog : Form
    {
        private readonly TextBox _brand = Box(); private readonly TextBox _type = Box(); private readonly TextBox _color = Box();
        private readonly TextBox _battery = Box(); private readonly TextBox _motor = Box(); private readonly TextBox _speed = Box();
        private readonly TextBox _range = Box(); private readonly TextBox _weight = Box(); private readonly TextBox _safety = Box();
        private readonly TextBox _image = Box(); private readonly TextBox _description = new() { Width = 250, Height = 65, Multiline = true };
        private readonly NumericUpDown _price = new() { Maximum = 1_000_000_000, ThousandsSeparator = true, Width = 180 };
        private readonly CheckBox _featured = new() { Text = "Featured" }; private readonly CheckBox _active = new() { Text = "Active", Checked = true };
        public string Brand => _brand.Text.Trim().ToUpperInvariant(); public string Type => _type.Text.Trim().ToUpperInvariant();
        public string Color => _color.Text.Trim().ToUpperInvariant(); public string Battery => _battery.Text.Trim(); public string Motor => _motor.Text.Trim();
        public string TopSpeed => _speed.Text.Trim(); public string Range => _range.Text.Trim(); public string MaxWeight => _weight.Text.Trim();
        public string Safety => _safety.Text.Trim(); public string ImagePath => _image.Text.Trim(); public string Description => _description.Text.Trim();
        public decimal SellPrice => _price.Value; public bool Featured => _featured.Checked; public bool Active => _active.Checked;

        public CatalogueEditDialog(DataRowView? row = null)
        {
            Text = row == null ? "Add bicycle" : "Edit bicycle"; StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(620, 600); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
            _brand.CharacterCasing = _type.CharacterCasing = _color.CharacterCasing = CharacterCasing.Upper;
            var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(16), AutoScroll = true };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Add(table, "Brand *", _brand); Add(table, "Type / model *", _type); Add(table, "Colour", _color); Add(table, "Selling price", _price);
            Add(table, "Battery", _battery); Add(table, "Motor", _motor); Add(table, "Top speed", _speed); Add(table, "Range", _range);
            Add(table, "Max weight", _weight); Add(table, "Safety", _safety); Add(table, "Image path", _image); Add(table, "Description", _description);
            var flags = new FlowLayoutPanel { AutoSize = true }; flags.Controls.Add(_featured); flags.Controls.Add(_active); Add(table, "Visibility", flags);
            var actions = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
            var ok = new Button { Text = "Save", Width = 90, DialogResult = DialogResult.OK }; var cancel = new Button { Text = "Cancel", Width = 90, DialogResult = DialogResult.Cancel };
            ok.Click += (_, e) => { if (string.IsNullOrWhiteSpace(_brand.Text) || string.IsNullOrWhiteSpace(_type.Text)) { MessageBox.Show("Brand and type/model are required."); DialogResult = DialogResult.None; } };
            actions.Controls.Add(ok); actions.Controls.Add(cancel); Add(table, "", actions); Controls.Add(table); AcceptButton = ok; CancelButton = cancel;
            if (row != null) LoadRow(row);
        }
        private void LoadRow(DataRowView r)
        {
            _brand.Text = Convert.ToString(r["brand"]); _type.Text = Convert.ToString(r["type"]); _color.Text = Convert.ToString(r["color"]);
            _price.Value = Math.Min(_price.Maximum, Convert.ToDecimal(r["sell_price"])); _battery.Text = Convert.ToString(r["battery"]);
            _motor.Text = Convert.ToString(r["motor"]); _speed.Text = Convert.ToString(r["top_speed"]); _range.Text = Convert.ToString(r["range_text"]);
            _weight.Text = Convert.ToString(r["max_weight"]); _safety.Text = Convert.ToString(r["safety"]); _image.Text = Convert.ToString(r["image_path"]);
            _description.Text = Convert.ToString(r["description"]); _featured.Checked = Convert.ToInt32(r["featured"]) == 1; _active.Checked = Convert.ToInt32(r["is_active"]) == 1;
        }
        private static TextBox Box() => new() { Width = 250 };
        private static void Add(TableLayoutPanel table, string label, Control control)
        {
            var row = table.RowCount++; table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            table.Controls.Add(new Label { Text = label, AutoSize = true, Padding = new Padding(0, 7, 0, 0) }, 0, row); table.Controls.Add(control, 1, row);
        }
    }
}
