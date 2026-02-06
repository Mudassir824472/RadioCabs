using System.Collections.Generic;

namespace RadioCabs.Models
{
    public class ListingPageViewModel
    {
        public Company Registration { get; set; } = new Company();
        public List<Company> Results { get; set; } = new List<Company>();
        public string SearchTerm { get; set; } = string.Empty;
    }
}
