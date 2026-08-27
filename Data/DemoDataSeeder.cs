using System;
using System.Collections.Generic;

namespace Bike_STore_Project
{
    public static class DemoDataSeeder
    {
        public static void SeedIfEmpty()
        {
            using (var connection = Database.OpenConnection())
            using (var count = connection.CreateCommand())
            {
                count.CommandText = "SELECT COUNT(*) FROM bikes;";
                if (Convert.ToInt32(count.ExecuteScalar() ?? 0) > 0) return;

                using var brands = connection.CreateCommand();
                brands.CommandText = @"
INSERT OR IGNORE INTO brands(name,sort_order) VALUES('VOLTERRA',10);
INSERT OR IGNORE INTO brands(name,sort_order) VALUES('URBANRIDE',20);
INSERT OR IGNORE INTO brands(name,sort_order) VALUES('E-MOTION',30);";
                brands.ExecuteNonQuery();
            }

            var brandIds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (var connection = Database.OpenConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT CAST(id AS TEXT),name FROM brands;";
                using var reader = command.ExecuteReader();
                while (reader.Read()) brandIds[reader.GetString(1)] = reader.GetString(0);
            }

            var repository = new WebsiteBikeRepository();
            repository.SaveBike(new WebsiteBike
            {
                Id = "volterra-city-s1", BrandId = brandIds["VOLTERRA"], Brand = "VOLTERRA", Name = "CITY S1",
                Battery = "48V 20Ah", Motor = "500W", TopSpeed = "35 km/h", Range = "55–70 km", MaxWeight = "150 kg",
                Description = "Comfortable commuter bicycle for demonstrating catalogue, colour and stock workflows.", Price = 15900000,
                Featured = true, InStock = true, Colors = new()
                {
                    new WebsiteBikeColor { Name = "MIDNIGHT BLUE", Hex = "#223a5e", StockQty = 6 },
                    new WebsiteBikeColor { Name = "PEARL WHITE", Hex = "#f1f1ed", StockQty = 4 }
                }
            }, true);
            repository.SaveBike(new WebsiteBike
            {
                Id = "urbanride-fold-x2", BrandId = brandIds["URBANRIDE"], Brand = "URBANRIDE", Name = "FOLD X2",
                Battery = "48V 15Ah", Motor = "350W", TopSpeed = "30 km/h", Range = "40–55 km", MaxWeight = "120 kg",
                Description = "Compact folding model with independent stock for each colour.", Price = 12450000,
                Featured = true, InStock = true, Colors = new()
                {
                    new WebsiteBikeColor { Name = "GRAPHITE", Hex = "#42474d", StockQty = 5 },
                    new WebsiteBikeColor { Name = "SAGE GREEN", Hex = "#839b86", StockQty = 3 },
                    new WebsiteBikeColor { Name = "SUNSET ORANGE", Hex = "#d66b35", StockQty = 2 }
                }
            }, true);
            repository.SaveBike(new WebsiteBike
            {
                Id = "e-motion-cargo-pro", BrandId = brandIds["E-MOTION"], Brand = "E-MOTION", Name = "CARGO PRO",
                Battery = "60V 24Ah", Motor = "800W", TopSpeed = "32 km/h", Range = "60–80 km", MaxWeight = "220 kg",
                Description = "Utility model included to demonstrate low-stock warnings and replenishment.", Price = 21900000,
                InStock = true, Colors = new()
                {
                    new WebsiteBikeColor { Name = "CHARCOAL", Hex = "#34383b", StockQty = 2 }
                }
            }, true);
        }
    }
}
