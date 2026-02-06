namespace RadioCabs.Models
{
    public class AdminDashboardViewModel
    {
        public List<Company> Companies { get; set; } = new();
        public List<Driver> Drivers { get; set; } = new();
        public List<Advertisement> Advertisements { get; set; } = new();
        public List<Feedback> Feedbacks { get; set; } = new();
    }
}
