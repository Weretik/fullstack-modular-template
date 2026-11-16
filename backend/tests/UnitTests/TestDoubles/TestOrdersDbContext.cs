using Microsoft.EntityFrameworkCore;
using Orders.Application;
using Orders.Domain.Orders;

namespace UnitTests.TestDoubles;

internal sealed class TestOrdersDbContext() : DbContext(
    new DbContextOptionsBuilder<TestOrdersDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options), IOrdersDbContext
{
    public List<object> OutboxMessages { get; } = [];

    public DbSet<Order> Orders => Set<Order>();

    public void AddOutboxMessage(object message) => OutboxMessages.Add(message);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(order =>
        {
            order.Ignore(o => o.Version);
            order.OwnsMany(o => o.Lines, lines =>
            {
                lines.WithOwner().HasForeignKey("OrderId");
                lines.Property<int>("Id");
                lines.HasKey("Id");
                lines.Ignore(l => l.LineTotal);
            });
            order.Navigation(o => o.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }
}
