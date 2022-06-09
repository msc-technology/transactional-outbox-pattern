using MeetupDemo.Carts.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetupDemo.Carts.DataAccess;

public class CartDbContext : DbContext
{
    public DbSet<CustomerAvailabilityFund> AvailabilityFunds { get; set; }
    public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CustomerAvailabilityFund
        modelBuilder.Entity<CustomerAvailabilityFund>().ToTable("CustomerAvailabilityFunds");
        modelBuilder.Entity<CustomerAvailabilityFund>().HasKey(m => m.Id);
        modelBuilder.Entity<CustomerAvailabilityFund>()
            .Property(m => m.Currency).HasColumnType("VARCHAR").HasMaxLength(3);
    }
}
