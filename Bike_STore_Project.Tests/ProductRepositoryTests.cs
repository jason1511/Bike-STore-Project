using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Bike_STore_Project;

namespace Bike_STore_Project.Tests
{
    [TestClass]
    public class ProductRepositoryTests
    {
        private string _dbPath = string.Empty;

        [TestInitialize]
        public void Setup()
        {
            _dbPath = Path.Combine(Path.GetTempPath(), $"bike_store_test_{Guid.NewGuid()}.db");
            Database.UseDatabaseFile(_dbPath);
            Database.Initialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                if (File.Exists(_dbPath))
                    File.Delete(_dbPath);
            }
            catch { /* ignore */ }
        }

        [TestMethod]
        public void Insert_Then_GetAll_ReturnsInserted()
        {
            var repo = new ProductRepository();

            repo.Insert(new Product
            {
                Brand = "Giant",
                Type = "E-Bike",
                Color = "Black",
                Quantity = 3,
                Price = 1500m
            });

            var all = repo.GetAll();
            Assert.AreEqual(1, all.Count);
            Assert.AreEqual("Giant", all[0].Brand);
            Assert.AreEqual("E-Bike", all[0].Type);
            Assert.AreEqual(3, all[0].Quantity);
        }

        [TestMethod]
        public void Update_ChangesValues()
        {
            var repo = new ProductRepository();

            var id = repo.Insert(new Product
            {
                Brand = "Trek",
                Type = "Helmet",
                Color = "Red",
                Quantity = 10,
                Price = 79.99m
            });

            var p = repo.GetById(id)!;
            p.Quantity = 8;
            p.Price = 69.99m;

            var ok = repo.Update(p);
            Assert.IsTrue(ok);

            var updated = repo.GetById(id)!;
            Assert.AreEqual(8, updated.Quantity);
            Assert.AreEqual(69.99m, updated.Price);
        }

        [TestMethod]
        public void Delete_RemovesRow()
        {
            var repo = new ProductRepository();

            var id = repo.Insert(new Product
            {
                Brand = "KMC",
                Type = "Chain",
                Color = null,
                Quantity = 5,
                Price = 25m
            });

            Assert.IsNotNull(repo.GetById(id));

            var ok = repo.Delete(id);
            Assert.IsTrue(ok);

            Assert.IsNull(repo.GetById(id));
        }
    }
}
