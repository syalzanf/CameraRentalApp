using Microsoft.EntityFrameworkCore;
using CameraRentalApp.Models;

namespace CameraRentalApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Transaction> Transactions { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .ToTable("Rentals")
                .HasKey(t => t.RentalId);
        }
    }
}
