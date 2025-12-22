using System;
using System.Globalization;
using System.Windows.Forms;
using System.Threading;


namespace Bike_STore_Project
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Database.Initialize();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("id-ID");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("id-ID");
            Application.Run(new InventoryForm()); // start on inventory for now
        }
    }
}
