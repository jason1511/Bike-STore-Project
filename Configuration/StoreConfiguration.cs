using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bike_STore_Project
{
    public enum StoreBackendMode
    {
        Demo,
        Local,
        Cloudflare
    }

    public sealed class StoreProfile
    {
        public string ProfileName { get; set; } = "Demo Store";
        public StoreBackendMode Backend { get; set; } = StoreBackendMode.Demo;
        public string ApiBaseUrl { get; set; } = "";
        public string DatabasePath { get; set; } = "";
        public string StoreName { get; set; } = "Electric Bike Store";
        public string ShortName { get; set; } = "EBS";
        public string CurrencyCode { get; set; } = "IDR";
        public string Culture { get; set; } = "id-ID";
        public string InvoiceTitle { get; set; } = "FAKTUR PENJUALAN";
        public int LowStockThreshold { get; set; } = 5;

        public bool IsDemo => Backend == StoreBackendMode.Demo;
        public bool IsOnline => Backend == StoreBackendMode.Cloudflare;
        public string BackendLabel => Backend switch
        {
            StoreBackendMode.Demo => Strings.Get("Backend_Demo"),
            StoreBackendMode.Local => Strings.Get("Backend_Local"),
            _ => Strings.Get("Backend_Online")
        };

        public static StoreProfile CreateDemo() => new()
        {
            ProfileName = "Demo Store",
            Backend = StoreBackendMode.Demo,
            StoreName = "Electric Bike Store Demo",
            ShortName = "DEMO",
            DatabasePath = AppPaths.DemoDatabasePath
        };

        public static StoreProfile CreateLocal() => new()
        {
            ProfileName = "Local Store",
            Backend = StoreBackendMode.Local,
            StoreName = "My Electric Bike Store",
            ShortName = "EBS",
            DatabasePath = AppPaths.LocalDatabasePath
        };

        public static StoreProfile CreateCvNiaga() => new()
        {
            ProfileName = "CV Niaga Bersama",
            Backend = StoreBackendMode.Cloudflare,
            ApiBaseUrl = "https://niagabersama.com",
            StoreName = "CV Niaga Bersama Abadi",
            ShortName = "NBA",
            CurrencyCode = "IDR",
            Culture = "id-ID",
            InvoiceTitle = "FAKTUR PENJUALAN"
        };
    }

    public static class AppPaths
    {
        public static string DataDirectory { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BikeStoreDesktop");
        public static string SettingsPath => Path.Combine(DataDirectory, "store-profile.json");
        public static string DemoDatabasePath => Path.Combine(DataDirectory, "demo.db");
        public static string LocalDatabasePath => Path.Combine(DataDirectory, "store.db");

        public static void EnsureDataDirectory() => Directory.CreateDirectory(DataDirectory);
    }

    public static class StoreConfiguration
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        static StoreConfiguration() => JsonOptions.Converters.Add(new JsonStringEnumConverter());

        public static bool Exists => File.Exists(AppPaths.SettingsPath);

        public static StoreProfile Load() => Load(out _);

        public static StoreProfile Load(out string? error)
        {
            error = null;
            try
            {
                AppPaths.EnsureDataDirectory();
                if (!File.Exists(AppPaths.SettingsPath)) return StoreProfile.CreateDemo();
                return JsonSerializer.Deserialize<StoreProfile>(File.ReadAllText(AppPaths.SettingsPath), JsonOptions)
                    ?? throw new InvalidDataException(Strings.Get("Startup_InvalidSettings"));
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return StoreProfile.CreateDemo();
            }
        }

        public static void Save(StoreProfile profile)
        {
            AppPaths.EnsureDataDirectory();
            File.WriteAllText(AppPaths.SettingsPath, JsonSerializer.Serialize(profile, JsonOptions));
        }
    }
}
