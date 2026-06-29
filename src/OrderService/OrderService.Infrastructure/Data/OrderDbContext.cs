using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(order =>
        {
            order.HasKey(o => o.Id);
            order.Property(o => o.CustomerName).IsRequired().HasMaxLength(200);
            order.Property(o => o.CustomerEmail).IsRequired().HasMaxLength(200);
            order.Property(o => o.Status).HasConversion<string>().IsRequired();
            order.Property(o => o.CreatedAt).IsRequired();

            order.OwnsOne(o => o.TotalAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("TotalAmount").HasPrecision(18, 2);
                money.Property(m => m.Currency).HasColumnName("TotalAmountCurrency").HasMaxLength(3);
            });

            order.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId");
        });

        modelBuilder.Entity<OrderItem>(item =>
        {
            item.HasKey(i => i.Id);
            item.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
            item.Property(i => i.Quantity).IsRequired();

            item.OwnsOne(i => i.UnitPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("UnitPrice").HasPrecision(18, 2);
                money.Property(m => m.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3);
            });
        });
    }
}
