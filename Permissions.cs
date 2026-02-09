namespace Bike_STore_Project
{
    public static class Permissions
    {
        public static bool CanManageInventory => AppSession.IsAdmin;   // edit/delete/add lots
        public static bool CanMakeSales => AppSession.IsSignedIn;      // user+admin
        public static bool CanAddService => AppSession.IsSignedIn;     // user+admin
        public static bool CanManageUsers => AppSession.IsAdmin;       // admin only
    }
}
