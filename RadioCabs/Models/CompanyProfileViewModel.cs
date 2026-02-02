using System.ComponentModel.DataAnnotations;

namespace RadioCabs.Models
{
    public class CompanyProfileViewModel
    {
        public int CompanyId { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string CompanyUniqueId { get; set; } // display only

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? ContactPerson { get; set; }
        public string? Designation { get; set; }
        public string? Mobile { get; set; }
        public string? Telephone { get; set; }
        public string? FaxNumber { get; set; }
        public string? Address { get; set; }

        [Required]
        public string MembershipType { get; set; }

        [Required]
        public string PaymentType { get; set; }

        public string? PaymentStatus { get; set; }

        // Optional: show calculated amount
        public int PaymentAmount { get; set; }
    }
}
