using BuildingBlocks.Infrastructure.Outbox.Dispatch;
using BuildingBlocks.Infrastructure.Outbox.Storage;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Orders.Application;
using Orders.Domain.Orders;

namespace Orders.Infrastructure.Persistence;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options, OutboxMessageTypes outboxTypes)
    : ModuleDbContext(options, Schema), IOrdersDbContext
{
    public const string Schema = "orders";

    public DbSet<Order> Orders => Set<Order>();

    public void AddOutboxMessage(object message) => Set<OutboxMessage>().Add(outboxTypes.CreateRow(message));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}
