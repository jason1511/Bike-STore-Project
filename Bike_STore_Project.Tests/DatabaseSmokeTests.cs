using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bike_STore_Project;

namespace Bike_STore_Project.Tests
{
    [TestClass]
    public class DatabaseSmokeTests
    {
        [TestMethod]
        public void Database_Initialize_DoesNotThrow()
        {
            Database.UseDatabaseFile(":memory:");
            Database.Initialize();
        }
    }
}
