using System.Collections.Generic;

namespace RadioCabs.Models
{
    public class AdvertisePageViewModel
    {
        public Advertisement Advertisement { get; set; } = new Advertisement();
        public int? CompanyId { get; set; }
        public List<Company> Companies { get; set; } = new List<Company>();
    }
}
