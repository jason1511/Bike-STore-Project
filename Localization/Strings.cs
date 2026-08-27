using System.Globalization;
using System.Resources;

namespace Bike_STore_Project
{
    internal static class Strings
    {
        private static readonly ResourceManager Manager =
            new("Bike_STore_Project.Resources.Strings", typeof(Strings).Assembly);

        public static string Get(string key)
            => Manager.GetString(key, CultureInfo.CurrentUICulture) ?? $"[{key}]";

        public static bool TryGet(string key, out string value)
        {
            value = Manager.GetString(key, CultureInfo.CurrentUICulture) ?? "";
            return value.Length > 0;
        }

        public static string Format(string key, params object?[] args)
            => string.Format(CultureInfo.CurrentCulture, Get(key), args);

        public static string Status(string code)
        {
            var normalized = code.Trim().ToUpperInvariant();
            return TryGet("Status_" + normalized, out var value) ? value : code;
        }

        public static string Role(string code)
            => Get(code.Equals("ADMIN", System.StringComparison.OrdinalIgnoreCase) ? "Role_ADMIN" : "Role_USER");

        public static string Movement(string code)
            => TryGet("Movement_" + code.Trim().ToUpperInvariant(), out var value) ? value : code;
    }
}
