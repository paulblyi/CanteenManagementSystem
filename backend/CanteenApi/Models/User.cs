using System.ComponentModel.DataAnnotations;

namespace CanteenApi.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [EmailAddress]
        public string? Email { get; set; }
        
        [Required]
        public string Role { get; set; } = "Employee"; // Employee, HumanCapital, Chef, Admin
        
        [MaxLength(50)]
        public string? Department { get; set; }
        
        [MaxLength(20)]
        public string? EmployeeCode { get; set; }
        
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
