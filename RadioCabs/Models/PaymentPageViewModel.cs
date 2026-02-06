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
    }
}
