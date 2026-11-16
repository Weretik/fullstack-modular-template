namespace Catalog.Contracts;

/// <summary>A published product as another module sees it.</summary>
public sealed record ProductForSale(Guid Id, string Name, decimal Price);
