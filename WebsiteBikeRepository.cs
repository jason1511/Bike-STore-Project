using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Bike_STore_Project
{
    public sealed class WebsiteBikeColor
    {
        public string Name { get; set; } = "";
        public string Hex { get; set; } = "#cccccc";
        public string Image { get; set; } = "";
        public int StockQty { get; set; }
    }

    public sealed class WebsiteBike
    {
        public string Id { get; set; } = "";
        public string BrandId { get; set; } = "";
        public string Brand { get; set; } = "";
        public string Name { get; set; } = "";
        public string Battery { get; set; } = "";
        public string Motor { get; set; } = "";
        public string TopSpeed { get; set; } = "";
        public string Range { get; set; } = "";
        public string MaxWeight { get; set; } = "";
        public string Safety { get; set; } = "";
        public string Image { get; set; } = "";
        public string Alt { get; set; } = "";
        public string Comfort { get; set; } = "medium";
        public string ColorName { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public bool Featured { get; set; }
        public bool InStock { get; set; } = true;
        public List<WebsiteBikeColor> Colors { get; set; } = new();
        public int StockQty => Colors.Sum(x => Math.Max(0, x.StockQty));
        public string ColorSummary => string.Join(", ", Colors.Select(x => $"{x.Name} ({x.StockQty})"));
    }

    public sealed class WebsiteBikeRepository
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public List<WebsiteBike> GetAll(string? search = null)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT id,brand_id,brand,name,battery,motor,topSpeed,range,maxWeight,safety,image,alt,comfort,
       description,price,featured,inStock,stockQty,colors
FROM bikes
WHERE $q='' OR UPPER(brand) LIKE $like OR UPPER(name) LIKE $like OR UPPER(colors) LIKE $like
ORDER BY brand,name;";
            var q = search?.Trim().ToUpperInvariant() ?? "";
            cmd.Parameters.AddWithValue("$q", q); cmd.Parameters.AddWithValue("$like", $"%{q}%");
            using var reader = cmd.ExecuteReader();
            var result = new List<WebsiteBike>();
            while (reader.Read()) result.Add(ReadBike(reader));
            return result;
        }

        public WebsiteBike? GetById(string id)
        {
            using var conn = Database.OpenConnection();
            return GetById(conn, null, id);
        }

        public void SaveBike(WebsiteBike bike, bool isNew)
        {
            NormalizeAndValidate(bike);
            using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                var existing = GetById(conn, tx, bike.Id);
                if (isNew && existing != null) throw new InvalidOperationException("Bike ID already exists.");
                if (!isNew && existing == null) throw new InvalidOperationException("Bike not found.");
                if (!isNew && !AppSession.IsAdmin && existing!.InStock != bike.InStock)
                    throw new InvalidOperationException("Only an administrator can change catalogue status.");

                var previousColors = existing?.Colors ?? new List<WebsiteBikeColor>();
                for (var index = 0; index < bike.Colors.Count; index++)
                {
                    var color = bike.Colors[index];
                    var previous = previousColors.FirstOrDefault(x => Same(x.Name, color.Name))
                        ?? (index < previousColors.Count ? previousColors[index] : null);
                    var productId = EnsureProduct(conn, tx, bike, color, previous?.Name);
                    SetProductAvailableStock(conn, tx, productId, color.StockQty, bike, color,
                        isNew ? "Stok awal sepeda baru" : "Penyesuaian stok dari editor sepeda");
                }

                foreach (var removed in previousColors.Where(old => !bike.Colors.Any(next => Same(old.Name, next.Name))))
                {
                    var productId = FindProductId(conn, tx, bike.Brand, bike.Name, removed.Name);
                    if (productId > 0)
                    {
                        var shadow = new WebsiteBikeColor { Name = removed.Name, Hex = removed.Hex, Image = removed.Image, StockQty = 0 };
                        SetProductAvailableStock(conn, tx, productId, 0, bike, shadow, "Warna dihapus dari editor sepeda");
                        using var deactivate = conn.CreateCommand(); deactivate.Transaction = tx;
                        deactivate.CommandText = "UPDATE products SET is_active=0 WHERE id=$id;";
                        deactivate.Parameters.AddWithValue("$id", productId); deactivate.ExecuteNonQuery();
                    }
                }

                RefreshColorQuantities(conn, tx, bike);
                UpsertBike(conn, tx, bike, isNew);
                LocalAdminRepository.WriteAudit(conn, tx, isNew ? "CREATE_BIKE" : "UPDATE_BIKE", "bikes", null,
                    $"{bike.Brand} {bike.Name}; colors={bike.Colors.Count}; stock={bike.StockQty}");
                tx.Commit();
            }
            catch { try { tx.Rollback(); } catch { } throw; }
        }

        public void ReceiveStock(string bikeId, string colorName, string colorHex, string colorImage,
            int quantity, decimal unitCost, DateTime receivedAt, string note)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be at least 1.");
            if (unitCost <= 0) throw new ArgumentException("Unit cost must be greater than 0.");
            using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                var bike = GetById(conn, tx, bikeId) ?? throw new InvalidOperationException("Bike not found.");
                var color = bike.Colors.FirstOrDefault(x => Same(x.Name, colorName));
                if (color == null)
                {
                    color = new WebsiteBikeColor { Name = colorName.Trim(), Hex = NormalHex(colorHex), Image = colorImage.Trim() };
                    if (string.IsNullOrWhiteSpace(color.Name)) throw new ArgumentException("New colour name is required.");
                    bike.Colors.Add(color);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(colorHex)) color.Hex = NormalHex(colorHex);
                    if (!string.IsNullOrWhiteSpace(colorImage)) color.Image = colorImage.Trim();
                }

                var productId = EnsureProduct(conn, tx, bike, color, null);
                var before = AvailableStock(conn, tx, productId);
                long lotId;
                using (var lot = conn.CreateCommand())
                {
                    lot.Transaction = tx; lot.CommandText = @"
INSERT INTO stock_lots(product_id,received_at,unit_cost,qty_received,qty_remaining,notes)
VALUES($product,$at,$cost,$qty,$qty,$note); SELECT last_insert_rowid();";
                    lot.Parameters.AddWithValue("$product", productId); lot.Parameters.AddWithValue("$at", receivedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    lot.Parameters.AddWithValue("$cost", (double)unitCost); lot.Parameters.AddWithValue("$qty", quantity);
                    lot.Parameters.AddWithValue("$note", string.IsNullOrWhiteSpace(note) ? "Tambah stok" : note.Trim());
                    lotId = Convert.ToInt64(lot.ExecuteScalar() ?? 0L);
                }
                InsertMovement(conn, tx, productId, lotId, bike, color, "stock_in", quantity, before, before + quantity,
                    string.IsNullOrWhiteSpace(note) ? $"Penambahan stok - Warna {color.Name}" : note.Trim());
                color.StockQty = before + quantity;
                RefreshColorQuantities(conn, tx, bike); UpsertBike(conn, tx, bike, false);
                LocalAdminRepository.WriteAudit(conn, tx, "RECEIVE_STOCK", "stock_lots", lotId,
                    $"{bike.Brand} {bike.Name}; color={color.Name}; qty={quantity}; unit_cost={unitCost}");
                tx.Commit();
            }
            catch { try { tx.Rollback(); } catch { } throw; }
        }

        public void SetActive(string bikeId, bool active)
        {
            if (!AppSession.IsAdmin) throw new InvalidOperationException("Admin access required.");
            using var conn = Database.OpenConnection(); using var tx = conn.BeginTransaction();
            try
            {
                using var cmd = conn.CreateCommand(); cmd.Transaction = tx;
                cmd.CommandText = "UPDATE bikes SET inStock=$active,updatedAt=CURRENT_TIMESTAMP WHERE id=$id;";
                cmd.Parameters.AddWithValue("$active", active ? 1 : 0); cmd.Parameters.AddWithValue("$id", bikeId);
                if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Bike not found.");
                using var products = conn.CreateCommand(); products.Transaction = tx;
                products.CommandText = "UPDATE products SET is_active=$active WHERE UPPER(brand)=UPPER((SELECT brand FROM bikes WHERE id=$id)) AND UPPER(type)=UPPER((SELECT name FROM bikes WHERE id=$id));";
                products.Parameters.AddWithValue("$active", active ? 1 : 0); products.Parameters.AddWithValue("$id", bikeId); products.ExecuteNonQuery();
                LocalAdminRepository.WriteAudit(conn, tx, active ? "REACTIVATE_BIKE" : "DEACTIVATE_BIKE", "bikes", null, bikeId); tx.Commit();
            }
            catch { try { tx.Rollback(); } catch { } throw; }
        }

        public static void MigrateLegacyProducts(SqliteConnection conn)
        {
            using (var count = conn.CreateCommand())
            { count.CommandText = "SELECT COUNT(*) FROM bikes;"; if (Convert.ToInt32(count.ExecuteScalar() ?? 0) > 0) return; }

            var rows = new List<LegacyProduct>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
SELECT p.brand,p.type,COALESCE(p.color,''),COALESCE(SUM(l.qty_remaining),0),COALESCE(p.sell_price,0),
 COALESCE(p.battery,''),COALESCE(p.motor,''),COALESCE(p.top_speed,''),COALESCE(p.range_text,''),
 COALESCE(p.max_weight,''),COALESCE(p.safety,''),COALESCE(p.image_path,''),COALESCE(p.description,''),
 COALESCE(p.featured,0),COALESCE(p.is_active,1)
FROM products p LEFT JOIN stock_lots l ON l.product_id=p.id GROUP BY p.id ORDER BY p.brand,p.type,p.id;";
                using var reader = cmd.ExecuteReader();
                while (reader.Read()) rows.Add(new LegacyProduct(reader));
            }

            foreach (var group in rows.GroupBy(x => $"{x.Brand}\u001f{x.Name}", StringComparer.OrdinalIgnoreCase))
            {
                var first = group.First(); var id = UniqueSlug(conn, $"{first.Brand}-{first.Name}");
                var brandId = ""; using (var brand = conn.CreateCommand())
                { brand.CommandText = "SELECT CAST(id AS TEXT) FROM brands WHERE name=$name COLLATE NOCASE LIMIT 1;"; brand.Parameters.AddWithValue("$name", first.Brand); brandId = Convert.ToString(brand.ExecuteScalar()) ?? ""; }
                var bike = new WebsiteBike
                {
                    Id=id, BrandId=brandId, Brand=first.Brand, Name=first.Name, Battery=first.Battery, Motor=first.Motor,
                    TopSpeed=first.TopSpeed, Range=first.Range, MaxWeight=first.MaxWeight, Safety=first.Safety,
                    Image=first.Image, Alt=$"Sepeda listrik {first.Name}", Description=first.Description, Price=first.Price,
                    Featured=first.Featured, InStock=group.Any(x=>x.Active),
                    Colors=group.Select(x=>new WebsiteBikeColor { Name=string.IsNullOrWhiteSpace(x.Color)?"STANDARD":x.Color, Hex="#cccccc", Image=x.Image, StockQty=x.Stock }).ToList()
                };
                using var tx = conn.BeginTransaction(); UpsertBike(conn, tx, bike, true); tx.Commit();
            }

            using var snapshots = conn.CreateCommand(); snapshots.CommandText = @"
UPDATE stock_movements SET
 bike_brand=COALESCE(bike_brand,(SELECT brand FROM products WHERE id=product_id)),
 bike_name=COALESCE(bike_name,(SELECT type FROM products WHERE id=product_id)),
 bike_color_name=COALESCE(NULLIF(bike_color_name,''),(SELECT COALESCE(color,'') FROM products WHERE id=product_id)),
 bike_id=COALESCE(bike_id,(SELECT b.id FROM bikes b JOIN products p ON UPPER(p.brand)=UPPER(b.brand) AND UPPER(p.type)=UPPER(b.name) WHERE p.id=product_id LIMIT 1)),
 created_by_role=COALESCE(created_by_role,'SYSTEM');"; snapshots.ExecuteNonQuery();
        }

        public static void RefreshBikeStockForProduct(SqliteConnection conn, SqliteTransaction tx, int productId)
        {
            string brand, name;
            using (var product = conn.CreateCommand())
            {
                product.Transaction=tx; product.CommandText="SELECT brand,type FROM products WHERE id=$id;"; product.Parameters.AddWithValue("$id",productId);
                using var reader=product.ExecuteReader(); if(!reader.Read()) return; brand=reader.GetString(0); name=reader.GetString(1);
            }
            using var find=conn.CreateCommand(); find.Transaction=tx; find.CommandText="SELECT id FROM bikes WHERE UPPER(brand)=UPPER($brand) AND UPPER(name)=UPPER($name) LIMIT 1;";
            find.Parameters.AddWithValue("$brand",brand); find.Parameters.AddWithValue("$name",name); var id=Convert.ToString(find.ExecuteScalar()); if(string.IsNullOrWhiteSpace(id)) return;
            var bike=GetById(conn,tx,id); if(bike==null)return; RefreshColorQuantities(conn,tx,bike); UpsertBike(conn,tx,bike,false);
        }

        private static WebsiteBike ReadBike(SqliteDataReader r) => new()
        {
            Id=r.GetString(0), BrandId=r.IsDBNull(1)?"":r.GetString(1), Brand=r.GetString(2), Name=r.GetString(3),
            Battery=r.IsDBNull(4)?"":r.GetString(4), Motor=r.IsDBNull(5)?"":r.GetString(5), TopSpeed=r.IsDBNull(6)?"":r.GetString(6),
            Range=r.IsDBNull(7)?"":r.GetString(7), MaxWeight=r.IsDBNull(8)?"":r.GetString(8), Safety=r.IsDBNull(9)?"":r.GetString(9),
            Image=r.IsDBNull(10)?"":r.GetString(10), Alt=r.IsDBNull(11)?"":r.GetString(11), Comfort=r.IsDBNull(12)?"medium":r.GetString(12),
            Description=r.IsDBNull(13)?"":r.GetString(13), Price=Convert.ToDecimal(r.GetDouble(14)), Featured=r.GetInt32(15)==1,
            InStock=r.GetInt32(16)==1, Colors=ParseColors(r.IsDBNull(18)?"[]":r.GetString(18))
        };

        private static WebsiteBike? GetById(SqliteConnection conn, SqliteTransaction? tx, string id)
        {
            using var cmd=conn.CreateCommand(); cmd.Transaction=tx; cmd.CommandText=@"SELECT id,brand_id,brand,name,battery,motor,topSpeed,range,maxWeight,safety,image,alt,comfort,description,price,featured,inStock,stockQty,colors FROM bikes WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id",id); using var reader=cmd.ExecuteReader(); return reader.Read()?ReadBike(reader):null;
        }

        private static List<WebsiteBikeColor> ParseColors(string json)
        { try { return JsonSerializer.Deserialize<List<WebsiteBikeColor>>(json,JsonOptions)??new(); } catch { return new(); } }

        private static void NormalizeAndValidate(WebsiteBike bike)
        {
            bike.Brand=bike.Brand.Trim().ToUpperInvariant(); bike.Name=bike.Name.Trim().ToUpperInvariant();
            if(string.IsNullOrWhiteSpace(bike.Brand)||string.IsNullOrWhiteSpace(bike.Name)) throw new ArgumentException("Brand and model are required.");
            if(string.IsNullOrWhiteSpace(bike.Id)) bike.Id=Slug($"{bike.Brand}-{bike.Name}");
            bike.Colors=bike.Colors.Where(x=>!string.IsNullOrWhiteSpace(x.Name)).Select(x=>new WebsiteBikeColor{Name=x.Name.Trim().ToUpperInvariant(),Hex=NormalHex(x.Hex),Image=x.Image.Trim(),StockQty=Math.Max(0,x.StockQty)}).ToList();
            if(bike.Colors.Count==0) throw new ArgumentException("At least one colour is required.");
            if(bike.Colors.GroupBy(x=>x.Name,StringComparer.OrdinalIgnoreCase).Any(x=>x.Count()>1)) throw new ArgumentException("Colour names must be unique within a bike.");
        }

        private static int EnsureProduct(SqliteConnection conn, SqliteTransaction tx, WebsiteBike bike, WebsiteBikeColor color, string? previousName)
        {
            var id=FindProductId(conn,tx,bike.Brand,bike.Name,color.Name);
            if(id==0&&!string.IsNullOrWhiteSpace(previousName))
            {
                id=FindProductId(conn,tx,bike.Brand,bike.Name,previousName);
                if(id>0){using var rename=conn.CreateCommand();rename.Transaction=tx;rename.CommandText="UPDATE products SET color=$color WHERE id=$id;";rename.Parameters.AddWithValue("$color",color.Name);rename.Parameters.AddWithValue("$id",id);rename.ExecuteNonQuery();}
            }
            if(id==0)
            {
                using var insert=conn.CreateCommand();insert.Transaction=tx;insert.CommandText=@"INSERT INTO products(brand,type,color,quantity,price,is_active,sell_price) VALUES($brand,$name,$color,0,0,1,$price); SELECT last_insert_rowid();";
                insert.Parameters.AddWithValue("$brand",bike.Brand);insert.Parameters.AddWithValue("$name",bike.Name);insert.Parameters.AddWithValue("$color",color.Name);insert.Parameters.AddWithValue("$price",(double)bike.Price);id=Convert.ToInt32(insert.ExecuteScalar()??0);
            }
            using var update=conn.CreateCommand();update.Transaction=tx;update.CommandText=@"UPDATE products SET brand=$brand,type=$name,color=$color,battery=$battery,motor=$motor,top_speed=$speed,range_text=$range,max_weight=$weight,safety=$safety,image_path=$image,description=$description,featured=$featured,is_active=$active,sell_price=$price WHERE id=$id;";
            update.Parameters.AddWithValue("$brand",bike.Brand);update.Parameters.AddWithValue("$name",bike.Name);update.Parameters.AddWithValue("$color",color.Name);update.Parameters.AddWithValue("$battery",bike.Battery);update.Parameters.AddWithValue("$motor",bike.Motor);update.Parameters.AddWithValue("$speed",bike.TopSpeed);update.Parameters.AddWithValue("$range",bike.Range);update.Parameters.AddWithValue("$weight",bike.MaxWeight);update.Parameters.AddWithValue("$safety",bike.Safety);update.Parameters.AddWithValue("$image",string.IsNullOrWhiteSpace(color.Image)?bike.Image:color.Image);update.Parameters.AddWithValue("$description",bike.Description);update.Parameters.AddWithValue("$featured",bike.Featured?1:0);update.Parameters.AddWithValue("$active",bike.InStock?1:0);update.Parameters.AddWithValue("$price",(double)bike.Price);update.Parameters.AddWithValue("$id",id);update.ExecuteNonQuery();return id;
        }

        private static int FindProductId(SqliteConnection conn,SqliteTransaction tx,string brand,string name,string color)
        {using var cmd=conn.CreateCommand();cmd.Transaction=tx;cmd.CommandText="SELECT id FROM products WHERE UPPER(brand)=UPPER($brand) AND UPPER(type)=UPPER($name) AND UPPER(COALESCE(color,''))=UPPER($color) LIMIT 1;";cmd.Parameters.AddWithValue("$brand",brand);cmd.Parameters.AddWithValue("$name",name);cmd.Parameters.AddWithValue("$color",color);return Convert.ToInt32(cmd.ExecuteScalar()??0);}
        private static int AvailableStock(SqliteConnection conn,SqliteTransaction tx,int productId)
        {using var cmd=conn.CreateCommand();cmd.Transaction=tx;cmd.CommandText="SELECT COALESCE(SUM(qty_remaining),0) FROM stock_lots WHERE product_id=$id;";cmd.Parameters.AddWithValue("$id",productId);return Convert.ToInt32(cmd.ExecuteScalar()??0);}

        private static void SetProductAvailableStock(SqliteConnection conn,SqliteTransaction tx,int productId,int desired,WebsiteBike bike,WebsiteBikeColor color,string note)
        {
            var before=AvailableStock(conn,tx,productId);var change=desired-before;if(change==0)return;long? lotId=null;
            if(change>0)
            {
                decimal cost=bike.Price>0?bike.Price:0.01m;using(var latest=conn.CreateCommand()){latest.Transaction=tx;latest.CommandText="SELECT unit_cost FROM stock_lots WHERE product_id=$id ORDER BY datetime(received_at) DESC,id DESC LIMIT 1;";latest.Parameters.AddWithValue("$id",productId);var v=latest.ExecuteScalar();if(v!=null&&v!=DBNull.Value)cost=Convert.ToDecimal(v,CultureInfo.InvariantCulture);}
                using var lot=conn.CreateCommand();lot.Transaction=tx;lot.CommandText="INSERT INTO stock_lots(product_id,received_at,unit_cost,qty_received,qty_remaining,notes) VALUES($id,CURRENT_TIMESTAMP,$cost,$qty,$qty,$note); SELECT last_insert_rowid();";lot.Parameters.AddWithValue("$id",productId);lot.Parameters.AddWithValue("$cost",(double)cost);lot.Parameters.AddWithValue("$qty",change);lot.Parameters.AddWithValue("$note",note);lotId=Convert.ToInt64(lot.ExecuteScalar()??0L);
            }
            else
            {
                var remaining=-change;using var lots=conn.CreateCommand();lots.Transaction=tx;lots.CommandText="SELECT id,qty_remaining FROM stock_lots WHERE product_id=$id AND qty_remaining>0 ORDER BY datetime(received_at) DESC,id DESC;";lots.Parameters.AddWithValue("$id",productId);var rows=new List<(long Id,int Qty)>();using(var reader=lots.ExecuteReader()){while(reader.Read())rows.Add((reader.GetInt64(0),reader.GetInt32(1)));}
                foreach(var row in rows){if(remaining==0)break;var take=Math.Min(remaining,row.Qty);using var update=conn.CreateCommand();update.Transaction=tx;update.CommandText="UPDATE stock_lots SET qty_remaining=qty_remaining-$take WHERE id=$id;";update.Parameters.AddWithValue("$take",take);update.Parameters.AddWithValue("$id",row.Id);update.ExecuteNonQuery();remaining-=take;lotId??=row.Id;}
                if(remaining>0)throw new InvalidOperationException("Stock correction exceeds available stock.");
            }
            InsertMovement(conn,tx,productId,lotId,bike,color,change>0?"stock_in":"adjustment",change,before,desired,note);
        }

        private static void InsertMovement(SqliteConnection conn,SqliteTransaction tx,int productId,long? lotId,WebsiteBike bike,WebsiteBikeColor color,string type,int change,int before,int after,string note)
        {using var cmd=conn.CreateCommand();cmd.Transaction=tx;cmd.CommandText=@"INSERT INTO stock_movements(product_id,stock_lot_id,movement_type,quantity_change,quantity_before,quantity_after,note,created_by_user_id,created_by_username,created_at,bike_id,bike_brand,bike_name,bike_color_name,created_by_role) VALUES($product,$lot,$type,$change,$before,$after,$note,$uid,$user,$at,$bike,$brand,$name,$color,$role);";cmd.Parameters.AddWithValue("$product",productId);cmd.Parameters.AddWithValue("$lot",lotId.HasValue?lotId.Value:(object)DBNull.Value);cmd.Parameters.AddWithValue("$type",type);cmd.Parameters.AddWithValue("$change",change);cmd.Parameters.AddWithValue("$before",before);cmd.Parameters.AddWithValue("$after",after);cmd.Parameters.AddWithValue("$note",note);cmd.Parameters.AddWithValue("$uid",AppSession.UserId>0?AppSession.UserId:(object)DBNull.Value);cmd.Parameters.AddWithValue("$user",AppSession.Username);cmd.Parameters.AddWithValue("$at",DateTime.UtcNow.ToString("o"));cmd.Parameters.AddWithValue("$bike",bike.Id);cmd.Parameters.AddWithValue("$brand",bike.Brand);cmd.Parameters.AddWithValue("$name",bike.Name);cmd.Parameters.AddWithValue("$color",color.Name);cmd.Parameters.AddWithValue("$role",AppSession.Role);cmd.ExecuteNonQuery();}

        private static void RefreshColorQuantities(SqliteConnection conn,SqliteTransaction tx,WebsiteBike bike)
        {foreach(var color in bike.Colors){var id=FindProductId(conn,tx,bike.Brand,bike.Name,color.Name);color.StockQty=id==0?0:AvailableStock(conn,tx,id);}}
        private static void UpsertBike(SqliteConnection conn,SqliteTransaction tx,WebsiteBike bike,bool isNew)
        {var colors=JsonSerializer.Serialize(bike.Colors,JsonOptions);using var cmd=conn.CreateCommand();cmd.Transaction=tx;cmd.CommandText=@"INSERT INTO bikes(id,brand_id,brand,name,battery,motor,topSpeed,range,maxWeight,safety,image,alt,comfort,colorName,colors,description,price,featured,inStock,stockQty) VALUES($id,$brandId,$brand,$name,$battery,$motor,$speed,$range,$weight,$safety,$image,$alt,$comfort,$primary,$colors,$description,$price,$featured,$active,$stock) ON CONFLICT(id) DO UPDATE SET brand_id=excluded.brand_id,brand=excluded.brand,name=excluded.name,battery=excluded.battery,motor=excluded.motor,topSpeed=excluded.topSpeed,range=excluded.range,maxWeight=excluded.maxWeight,safety=excluded.safety,image=excluded.image,alt=excluded.alt,comfort=excluded.comfort,colorName=excluded.colorName,colors=excluded.colors,description=excluded.description,price=excluded.price,featured=excluded.featured,inStock=excluded.inStock,stockQty=excluded.stockQty,updatedAt=CURRENT_TIMESTAMP;";cmd.Parameters.AddWithValue("$id",bike.Id);cmd.Parameters.AddWithValue("$brandId",bike.BrandId);cmd.Parameters.AddWithValue("$brand",bike.Brand);cmd.Parameters.AddWithValue("$name",bike.Name);cmd.Parameters.AddWithValue("$battery",bike.Battery);cmd.Parameters.AddWithValue("$motor",bike.Motor);cmd.Parameters.AddWithValue("$speed",bike.TopSpeed);cmd.Parameters.AddWithValue("$range",bike.Range);cmd.Parameters.AddWithValue("$weight",bike.MaxWeight);cmd.Parameters.AddWithValue("$safety",bike.Safety);cmd.Parameters.AddWithValue("$image",bike.Image);cmd.Parameters.AddWithValue("$alt",string.IsNullOrWhiteSpace(bike.Alt)?$"Sepeda listrik {bike.Name}":bike.Alt);cmd.Parameters.AddWithValue("$comfort",bike.Comfort);cmd.Parameters.AddWithValue("$primary",bike.Colors.FirstOrDefault()?.Name??"");cmd.Parameters.AddWithValue("$colors",colors);cmd.Parameters.AddWithValue("$description",bike.Description);cmd.Parameters.AddWithValue("$price",(double)bike.Price);cmd.Parameters.AddWithValue("$featured",bike.Featured?1:0);cmd.Parameters.AddWithValue("$active",bike.InStock?1:0);cmd.Parameters.AddWithValue("$stock",bike.StockQty);cmd.ExecuteNonQuery();}

        private static bool Same(string a,string b)=>a.Trim().Equals(b.Trim(),StringComparison.OrdinalIgnoreCase);
        private static string NormalHex(string value)=>Regex.IsMatch(value??"", "^#[0-9a-fA-F]{6}$")?value:"#cccccc";
        private static string Slug(string value)=>Regex.Replace(value.Trim().ToLowerInvariant(),"[^a-z0-9]+","-").Trim('-');
        private static string UniqueSlug(SqliteConnection conn,string value){var baseId=Slug(value);var id=baseId;var n=2;while(true){using var cmd=conn.CreateCommand();cmd.CommandText="SELECT COUNT(*) FROM bikes WHERE id=$id;";cmd.Parameters.AddWithValue("$id",id);if(Convert.ToInt32(cmd.ExecuteScalar()??0)==0)return id;id=$"{baseId}-{n++}";}}

        private sealed class LegacyProduct
        {
            public string Brand,Name,Color,Battery,Motor,TopSpeed,Range,MaxWeight,Safety,Image,Description; public int Stock; public decimal Price; public bool Featured,Active;
            public LegacyProduct(SqliteDataReader r){Brand=r.GetString(0);Name=r.GetString(1);Color=r.GetString(2);Stock=r.GetInt32(3);Price=Convert.ToDecimal(r.GetDouble(4));Battery=r.GetString(5);Motor=r.GetString(6);TopSpeed=r.GetString(7);Range=r.GetString(8);MaxWeight=r.GetString(9);Safety=r.GetString(10);Image=r.GetString(11);Description=r.GetString(12);Featured=r.GetInt32(13)==1;Active=r.GetInt32(14)==1;}
        }
    }
}
