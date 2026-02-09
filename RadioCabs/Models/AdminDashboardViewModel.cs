using System.Collections.Generic;

namespace RadioCabs.Models
{
    public class AdminDashboardViewModel
    {
        // Collections for displaying data
        public List<Company> Companies { get; set; } = new List<Company>();
        public List<Driver> Drivers { get; set; } = new List<Driver>();
        public List<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        // Dashboard Statistics
        public int TotalCompanies { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalAdvertisements { get; set; }
        public int TotalFeedbacks { get; set; }

        // Payment Statistics
        public int PendingPayments { get; set; }
        public int CompletedPayments { get; set; }
        public decimal TotalRevenue { get; set; }

        // Membership Statistics
        public int PremiumMembers { get; set; }
        public int BasicMembers { get; set; }
        public int FreeMembers { get; set; }
    }
}