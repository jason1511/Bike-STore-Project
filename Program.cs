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

            var hadSettings = StoreConfiguration.Exists;
            var profile = StoreConfiguration.Load();
            if (!hadSettings)
            {
                using var setup = new StoreSetupForm();
                if (setup.ShowDialog() != DialogResult.OK) return;
                profile = setup.SelectedProfile;
            }

            try
            {
                Configure(profile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("The selected store profile could not be opened.\n\n" + ex.Message,
                    "Bike Store Desktop", MessageBoxButtons.OK, MessageBoxIcon.Error);
                using var setup = new StoreSetupForm(profile);
                if (setup.ShowDialog() != DialogResult.OK) return;
                profile = setup.SelectedProfile;
                Configure(profile);
            }

            // --- LOGIN FLOW ---
            using (var login = new LoginForm())
            {
                // If login cancelled or failed → exit app
                if (login.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new AdminDashboardForm());
            AppServices.Backend.Dispose();
        }

        private static void Configure(StoreProfile profile)
        {
            var culture = CultureInfo.GetCultureInfo(profile.Culture);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (!profile.IsOnline)
            {
                Database.UseDatabaseFile(profile.DatabasePath);
                Database.Initialize();
                var userRepo = new UserRepository();
                userRepo.EnsureUsersSchemaAndSeed();
                if (profile.IsDemo) DemoDataSeeder.SeedIfEmpty();
            }
            AppServices.Configure(profile);
        }
    }
}
