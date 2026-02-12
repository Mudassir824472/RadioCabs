using System.ComponentModel.DataAnnotations;

namespace RadioCabs.Models
{
    public class DriverProfileEditViewModel
    {
        public int DriverId { get; set; }

        [Required]
        [Display(Name = "Driver ID")]
        public string DriverUniqueId { get; set; }

        [Required]
        [Display(Name = "Driver Name")]
        public string DriverName { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Range(0, 60)]
        public int Experience { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [MinLength(6)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
