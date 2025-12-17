using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Bike_STore_Project;

namespace Bike_STore_Project.Tests
{
    [TestClass]
    public class Test1
    {
        private string _dbPath = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            _dbPath = Path.Combine(Path.GetTempPath(), $"bike_store_test_{Guid.NewGuid()}.db");
            Database.UseDatabaseFile(_dbPath);
            Database.Initialize();
        }

        [TestMethod]
        public void GetAll_WithSearch_FiltersCorrectly()
        {
            var repo = new ProductRepository();
            repo.Insert(new Product { Brand = "Giant", Type = "E-Bike", Color = "Black", Quantity = 1, Price = 1000m });
            repo.Insert(new Product { Brand = "Trek", Type = "Helmet", Color = "Red", Quantity = 1, Price = 50m });

            var filtered = repo.GetAll("Trek");
            Assert.AreEqual(1, filtered.Count);
            Assert.AreEqual("Trek", filtered[0].Brand);
        }
        [TestMethod]
        public void Insert_AllowsNullColor()
        {
            var repo = new ProductRepository();

            var id = repo.Insert(new Product
            {
                Brand = "KMC",
                Type = "Chain",
                Color = null,          // important part
                Quantity = 2,
                Price = 25m
            });

            var p = repo.GetById(id);

            Assert.IsNotNull(p);
            Assert.IsNull(p!.Color);
        }

    }
}
