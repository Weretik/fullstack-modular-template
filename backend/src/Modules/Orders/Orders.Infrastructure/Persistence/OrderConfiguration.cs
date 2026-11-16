using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Orders;

namespace Orders.Infrastructure.Persistence;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.CustomerName).HasMaxLength(Order.CustomerNameMaxLength).IsRequired();
        builder.Property(o => o.Phone).HasMaxLength(20).IsRequired();
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.Total).HasPrecision(14, 2);
        builder.Property(o => o.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(o => o.RequestHash).HasMaxLength(64).IsRequired();
        // PostgreSQL's xmin system column is the row version: an update made on a stale copy affects no row.
        builder.Property(o => o.Version).IsRowVersion();

        // The idempotency key: a repeated or racing checkout cannot store a second order.
        builder.HasIndex(o => o.IdempotencyKey).IsUnique();
        builder.HasIndex(o => new { o.Status, o.PlacedAtUtc });

        builder.OwnsMany(o => o.Lines, lines =>
        {
            lines.ToTable("order_lines");
            lines.WithOwner().HasForeignKey("OrderId");
            lines.Property<int>("Id");
            lines.HasKey("Id");
            lines.Property(l => l.ProductName).HasMaxLength(200).IsRequired();
            lines.Property(l => l.UnitPrice).HasPrecision(12, 2);
            lines.Ignore(l => l.LineTotal);
        });
        builder.Navigation(o => o.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
