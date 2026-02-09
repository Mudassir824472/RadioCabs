using System.ComponentModel.DataAnnotations;

namespace RadioCabs.Models
{
    public class AdminUser
    {
        public int AdminUserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
