using System.ComponentModel.DataAnnotations;

namespace RadioCabs.Models
{
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }

        [Required]
        public string DriverUniqueId { get; set; }

        [Required]
        public string DriverName { get; set; }

        [Required]
        public string Password { get; set; }
        public string ContactPerson { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public int Experience { get; set; }
        public string? Description { get; set; }

        public string? PaymentType { get; set; }
        public string? PaymentStatus { get; set; }
        public int PaymentAmount { get; set; }

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
    }


}
