using System.Collections.Generic;

namespace RadioCabs.Models
{
    public class AdvertisePageViewModel
    {
        public Advertisement Advertisement { get; set; } = new Advertisement();
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyUniqueId { get; set; } = string.Empty;
        public List<Company> Companies { get; set; } = new List<Company>();
    }
}
