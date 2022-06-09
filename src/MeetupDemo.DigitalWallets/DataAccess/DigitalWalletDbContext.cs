using MeetupDemo.DigitalWallets.DataAccess.Models;
using MeetupDemo.Shared.Outbox;
using Microsoft.EntityFrameworkCore;

namespace MeetupDemo.DigitalWallets.DataAccess;

public class DigitalWalletDbContext : DbContext
{
    public DbSet<CustomerFund> Funds { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DigitalWalletDbContext(DbContextOptions<DigitalWalletDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CustomerFund
        modelBuilder.Entity<CustomerFund>().ToTable("CustomerFunds");
        modelBuilder.Entity<CustomerFund>().HasKey(m => m.Id);
        modelBuilder.Entity<CustomerFund>()
            .Property(m => m.Currency).HasColumnType("VARCHAR").HasMaxLength(3);
        modelBuilder.Entity<CustomerFund>()
            .Property(m => m.CustomerName).HasMaxLength(50);

        // OutboxMessage
        modelBuilder.Entity<OutboxMessage>().ToTable("OutboxMessages");
        modelBuilder.Entity<OutboxMessage>().HasKey(m => m.Id);
        modelBuilder.Entity<OutboxMessage>()
            .Property(m => m.Id).HasMaxLength(50);
        modelBuilder.Entity<OutboxMessage>()
            .Property(m => m.Exchange).HasMaxLength(50);
        modelBuilder.Entity<OutboxMessage>()
            .Property(m => m.RoutingKey).HasMaxLength(50);
    }
}

