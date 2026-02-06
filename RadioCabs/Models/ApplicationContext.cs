using Microsoft.EntityFrameworkCore;

namespace RadioCabs.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .HasIndex(c => c.CompanyUniqueId)
                .IsUnique();

            modelBuilder.Entity<Driver>()
                .HasIndex(d => d.DriverUniqueId)
                .IsUnique();
        }



    }
}
