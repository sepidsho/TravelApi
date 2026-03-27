using Microsoft.EntityFrameworkCore;
using TravelApi.Models; // Make sure this matches your namespace for Destination and Tour

namespace TravelApi.Data
{
    public class TravelDbContext : DbContext
    {
        public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options)
        {
        }

        // These properties represent our database tables
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Tour> Tours { get; set; }

        // Optional: Adding some default data (Seeding) so the database isn't empty at start
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Destination>().HasData(
                new Destination { Id = 1, Name = "Paris", Country = "France" },
                new Destination { Id = 2, Name = "Tokyo", Country = "Japan" }
            );

            modelBuilder.Entity<Tour>().HasData(
                new Tour { Id = 1, Title = "Spring in Paris", Price = 1500.00m, DurationDays = 5, DestinationId = 1 },
                new Tour { Id = 2, Title = "Tokyo Express", Price = 2500.00m, DurationDays = 7, DestinationId = 2 }
            );
        }
    }
}