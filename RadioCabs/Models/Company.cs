using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadioCabs.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string CompanyUniqueId { get; set; }

        [Required]
        public string Password { get; set; }

        public string ContactPerson { get; set; }
        public string Designation { get; set; }
        public string Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? FaxNumber { get; set; }
        public string? Address { get; set; }

        public string Email { get; set; }
        public string MembershipType { get; set; }
        public string PaymentType { get; set; }
        public string? PaymentStatus { get; set; }
        public int PaymentAmount { get; set; }
    }
}
