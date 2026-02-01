using System.ComponentModel.DataAnnotations;

namespace RadioCabs.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
