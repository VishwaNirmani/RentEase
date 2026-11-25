using Microsoft.EntityFrameworkCore;
using RentEase.API.Entities;

namespace RentEase.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; } 
    public DbSet<Property> Properties { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Property>()
            .Property(property => property.PropertyType)
            .HasConversion<string>();
        
        modelBuilder.Entity<Property>()
            .HasOne(property => property.Owner)
            .WithMany()
            .HasForeignKey(property => property.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<User>()
            .Property(user => user.Role)
            .HasConversion<string>();

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();
        modelBuilder.Entity<Booking>()
            .Property(booking => booking.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.Tenant)
            .WithMany()
            .HasForeignKey(booking => booking.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.Property)
            .WithMany()
            .HasForeignKey(booking => booking.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
    
}