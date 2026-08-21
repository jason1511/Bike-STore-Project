using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Database init (your existing logic)
            Database.Initialize();
            var userRepo = new UserRepository();
            userRepo.EnsureUsersSchemaAndSeed();

            // Culture (keep as-is)
            Thread.CurrentThread.CurrentCulture = new CultureInfo("id-ID");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("id-ID");

            // --- LOGIN FLOW ---
            using (var login = new LoginForm())
            {
                // If login cancelled or failed → exit app
                if (login.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new AdminDashboardForm());
        }
    }
}
