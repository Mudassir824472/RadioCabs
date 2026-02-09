using System.ComponentModel.DataAnnotations;

namespace RadioCabs.Models
{
    public class DriverLoginViewModel
    {
        [Required]
        [Display(Name = "Driver ID")]
        public string DriverUniqueId { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
