using System.Collections.Generic;

namespace RadioCabs.Models
{
    public class SearchResultsViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public List<Company> Companies { get; set; } = new List<Company>();
        public List<Driver> Drivers { get; set; } = new List<Driver>();
        public List<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
