namespace RadioCabs.Services
{
    public class PaymentCheckoutRequest
    {
        public string Section { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string SuccessUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }
}
