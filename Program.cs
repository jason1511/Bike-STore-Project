using System;
using System.Windows.Forms;

namespace WinFormsApp1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Ensure database and tables exist
            Database.Initialize();

            // For now you can decide which form to start with:
            Application.Run(new SalesForm());
            // later you might change this to: Application.Run(new InventoryForm());
        }
    }
}
