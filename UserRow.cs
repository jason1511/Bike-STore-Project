using System;

namespace Bike_STore_Project
{
    public sealed class UserRow
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Role { get; set; } = "USER";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
