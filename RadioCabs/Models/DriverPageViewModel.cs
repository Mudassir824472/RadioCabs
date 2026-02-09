using System.Collections.Generic;

namespace RadioCabs.Models
{
    public class DriverPageViewModel
    {
        public Driver Registration { get; set; } = new Driver();
        public List<Driver> Results { get; set; } = new List<Driver>();
        public string SearchTerm { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int? MinExperience { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
