using InventoryService.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<StockItem> StockItems => Set<StockItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockItem>(item =>
        {
            item.HasKey(i => i.ProductName);
            item.Property(i => i.ProductName).HasMaxLength(200);
            item.Property(i => i.Quantity).IsRequired();
            item.Property(i => i.UnitPrice).IsRequired().HasPrecision(18, 2);
            item.Property(i => i.Currency).IsRequired().HasMaxLength(3);
        });
    }
}
