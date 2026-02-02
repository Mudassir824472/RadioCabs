using System.ComponentModel.DataAnnotations;

namespace RadioCabs.Models
{
    public class CompanyLoginViewModel
    {
        [Required]
        public string CompanyUniqueId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
