using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Api.Models;

namespace TicketSystem.Api.Data
{
    public class TicketDbContext : IdentityDbContext<IdentityUser>
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<IdentityRole> Roles { get; set; } // Added for RoleManager

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required for Identity tables

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Music" },
                new Category { Id = 2, Name = "Sports" },
                new Category { Id = 3, Name = "Theater" }
            );
            modelBuilder.Entity<TicketCategory>().HasData(
                new TicketCategory { Id = 1, Name = "VIP", Price = 100.00m },
                new TicketCategory { Id = 2, Name = "Prestige", Price = 50.00m },
                new TicketCategory { Id = 3, Name = "Standard", Price = 20.00m }
            );
            modelBuilder.Entity<Event>().HasData(
                new Event { Id = 1, Name = "Rock Fest 2025", EventDate = new DateTime(2025, 3, 28), Venue = "City Arena", City = "New York", Description = "Epic rock night!", ImageUrl = "https://via.placeholder.com/150", CategoryId = 1 },
                new Event { Id = 2, Name = "Soccer Final", EventDate = new DateTime(2025, 3, 23), Venue = "Stadium X", City = "Los Angeles", Description = "Championship showdown.", ImageUrl = "https://via.placeholder.com/150", CategoryId = 2 },
                new Event { Id = 3, Name = "Hamlet Play", EventDate = new DateTime(2025, 3, 21), Venue = "Old Theater", City = "Chicago", Description = "Classic Shakespeare.", ImageUrl = "https://via.placeholder.com/150", CategoryId = 3 }
            );

            // Seed Tickets (50x10 grid)
            for (int eventId = 1; eventId <= 3; eventId++)
            {
                for (int row = 1; row <= 10; row++)
                {
                    for (int seat = 1; seat <= 50; seat++)
                    {
                        int categoryId = row <= 2 ? 1 : row <= 5 ? 2 : 3;
                        modelBuilder.Entity<Ticket>().HasData(
                            new Ticket { Id = (eventId - 1) * 500 + (row - 1) * 50 + seat, EventId = eventId, Row = row, Seat = seat, CategoryId = categoryId, IsSold = false }
                        );
                    }
                }
            }

            modelBuilder.Entity<TicketCategory>()
                .Property(tc => tc.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}