namespace Bike_STore_Project
{
    public static class Permissions
    {
        public static bool CanReceiveInventory => AppSession.IsSignedIn; // staff + admin
        public static bool CanEditInventory => AppSession.IsSignedIn;    // staff + admin
        public static bool CanDeleteInventory => AppSession.IsAdmin;     // admin only
        public static bool CanManageInventory => CanEditInventory;
        public static bool CanMakeSales => AppSession.IsSignedIn;      // user+admin
        public static bool CanAddService => AppSession.IsSignedIn;     // user+admin
        public static bool CanManageUsers => AppSession.IsAdmin;       // admin only
        public static bool CanViewReports => AppSession.IsAdmin;
        public static bool CanViewAudit => AppSession.IsAdmin;
        public static bool CanVoidInvoices => AppSession.IsAdmin;
    }
}
