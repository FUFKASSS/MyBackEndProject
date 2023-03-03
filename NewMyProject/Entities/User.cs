﻿namespace NewMyProject.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password {get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public long PhoneNumber { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public Profile? Profile { get; set; }
    }
}
