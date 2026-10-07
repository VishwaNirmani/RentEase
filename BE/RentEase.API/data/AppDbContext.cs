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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Property>()
            .HasOne(property => property.Owner)
            .WithMany()
            .HasForeignKey(property => property.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}