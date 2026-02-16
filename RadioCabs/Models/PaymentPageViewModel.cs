
using System.ComponentModel.DataAnnotations;

namespace RadioCabs.Models
{
    public class PaymentPageViewModel
    {
        public string Section { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PaymentType { get; set; } = "Monthly";
        public int PaymentAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";

        [Required(ErrorMessage = "Card holder name is required.")]
        [StringLength(80)]
        public string CardHolderName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Card number is required.")]
        [RegularExpression(@"^\d{13,19}$", ErrorMessage = "Card number must be 13 to 19 digits.")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "CVV is required.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
        public string Cvv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Card expiry month is required.")]
        [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12.")]
        public int? ExpiryMonth { get; set; }

        [Required(ErrorMessage = "Card expiry year is required.")]
        [Range(2026, 2100, ErrorMessage = "Enter a valid expiry year.")]
        public int? ExpiryYear { get; set; }
    }
}