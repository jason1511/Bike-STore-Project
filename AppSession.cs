namespace Bike_STore_Project
{
    public static class AppSession
    {
        public static int UserId { get; private set; }
        public static string Username { get; private set; } = "";
        public static string Role { get; private set; } = "USER";

        public static bool IsSignedIn => UserId > 0;
        public static bool IsAdmin => Role == "ADMIN";

        public static void SignIn(int id, string username, string role)
        {
            UserId = id;
            Username = username;
            Role = role;
        }

        public static void SignOut()
        {
            UserId = 0;
            Username = "";
            Role = "USER";
        }
    }
}
