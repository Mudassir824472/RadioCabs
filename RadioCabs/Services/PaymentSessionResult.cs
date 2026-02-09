using System.Collections.Generic;

namespace RadioCabs.Services
{
    public class PaymentSessionResult
    {
        public string SessionId { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
