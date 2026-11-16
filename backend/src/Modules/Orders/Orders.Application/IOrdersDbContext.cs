using Microsoft.EntityFrameworkCore;
using Orders.Domain.Orders;

namespace Orders.Application;

/// <summary>The Orders tables as the use cases see them, plus the module outbox.</summary>
public interface IOrdersDbContext
{
    DbSet<Order> Orders { get; }

    /// <summary>Stores a follow-up message; it is saved by the same <see cref="SaveChangesAsync"/> as the data.</summary>
    void AddOutboxMessage(object message);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
