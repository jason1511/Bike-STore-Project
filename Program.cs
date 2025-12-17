using System;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Database.Initialize();

            Application.Run(new InventoryForm()); // start on inventory for now
        }
    }
}
