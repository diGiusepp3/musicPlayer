﻿namespace MuziekApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsPremium { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}