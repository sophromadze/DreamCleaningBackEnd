using Microsoft.EntityFrameworkCore;
using DreamCleaningBackend.Models;

namespace DreamCleaningBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.FirstTimeOrder)
                .HasDefaultValue(true);

            // Apartment configuration
            modelBuilder.Entity<Apartment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Apartments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Subscription configuration
            modelBuilder.Entity<User>()
                .HasOne(u => u.Subscription)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.SubscriptionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed initial subscriptions
            modelBuilder.Entity<Subscription>().HasData(
                new Subscription
                {
                    Id = 1,
                    Name = "One-Time",
                    Description = "Single cleaning service",
                    FrequencyDays = 0,
                    DiscountPercentage = 0,
                    IsActive = true
                },
                new Subscription
                {
                    Id = 2,
                    Name = "Weekly",
                    Description = "Cleaning every week",
                    FrequencyDays = 7,
                    DiscountPercentage = 15,
                    IsActive = true
                },
                new Subscription
                {
                    Id = 3,
                    Name = "Bi-Weekly",
                    Description = "Cleaning every two weeks",
                    FrequencyDays = 14,
                    DiscountPercentage = 8,
                    IsActive = true
                },
                new Subscription
                {
                    Id = 4,
                    Name = "Monthly",
                    Description = "Cleaning once a month",
                    FrequencyDays = 30,
                    DiscountPercentage = 3,
                    IsActive = true
                }
            );
        }
    }
}