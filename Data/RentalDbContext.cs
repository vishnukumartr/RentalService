using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentalService.Models;
using static RentalService.Models.User;

namespace RentalService.Data
{
    public class RentalDbContext : DbContext
    {
        public RentalDbContext(DbContextOptions<RentalDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserEmail)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.IsSeller)
            .IsRequired();

            // Property entity
            modelBuilder.Entity<Property>()
                .HasKey(p => p.PropertyId);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.User)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade delete to avoid cycles

            // Interest entity
            modelBuilder.Entity<Interest>()
                .HasKey(i => i.InterestId);

            modelBuilder.Entity<Interest>()
                .HasOne(i => i.Property)
                .WithMany(p => p.Interests)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade delete to avoid cycles

            modelBuilder.Entity<Interest>()
                .HasOne(i => i.Buyer)
                .WithMany(u => u.Interests)
                .HasForeignKey(i => i.BuyerId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade delete to avoid cycles
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Interest> Interests { get; set; }
    }
}
