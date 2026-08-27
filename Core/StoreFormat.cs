using System.Globalization;

namespace Bike_STore_Project
{
    public static class StoreFormat
    {
        public static CultureInfo Culture
        {
            get
            {
                try { return CultureInfo.GetCultureInfo(AppServices.Profile.Culture); }
                catch { return CultureInfo.InvariantCulture; }
            }
        }

        public static string Money(decimal value) => value.ToString("C0", Culture);
        public static string ReportHeader => Strings.Format("Report_Header", AppServices.Profile.StoreName.ToUpperInvariant());
    }
}
