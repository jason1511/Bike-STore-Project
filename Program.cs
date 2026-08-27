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
            var profile = StoreConfiguration.Load(out var settingsError);
            if (!hadSettings || settingsError != null)
            {
                if (settingsError != null)
                    MessageBox.Show("The saved store settings could not be read. Choose the store connection again.\n\n" + settingsError,
                        "Store settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                using var setup = new StoreSetupForm();
                if (setup.ShowDialog() != DialogResult.OK) return;
                profile = setup.SelectedProfile;
            }

            string? initialAdminPassword = null;
            while (true)
            {
                try
                {
                    initialAdminPassword = Configure(profile);
                    break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The selected store profile could not be opened.\n\n" + ex.Message,
                        "Bike Store Desktop", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    using var setup = new StoreSetupForm(profile);
                    if (setup.ShowDialog() != DialogResult.OK) return;
                    profile = setup.SelectedProfile;
                }
            }

            // --- LOGIN FLOW ---
            using (var login = new LoginForm(initialAdminPassword))
            {
                // If login cancelled or failed → exit app
                if (login.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new AdminDashboardForm());
            AppServices.Backend.Dispose();
        }

        private static string? Configure(StoreProfile profile)
        {
            var culture = CultureInfo.GetCultureInfo(profile.Culture);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            string? initialAdminPassword = null;
            if (!profile.IsOnline)
            {
                Database.UseDatabaseFile(profile.DatabasePath);
                Database.Initialize();
                var userRepo = new UserRepository();
                initialAdminPassword = userRepo.EnsureUsersSchemaAndSeed();
                if (profile.IsDemo) DemoDataSeeder.SeedIfEmpty();
            }
            AppServices.Configure(profile);
            return initialAdminPassword;
        }
    }
}
