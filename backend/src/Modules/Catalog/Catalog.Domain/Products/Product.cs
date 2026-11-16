using BuildingBlocks.Domain.Modeling;
using BuildingBlocks.Domain.Results;

namespace Catalog.Domain.Products;

/// <summary>A product of the store. A new product is a draft until staff publish it.</summary>
public sealed class Product : Entity<Guid>, IAggregateRoot
{
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 4_000;

    private Product()
    {
    }

    public string Name { get; private set; } = string.Empty;

    /// <summary>Checked by <see cref="Domain.Slug"/> before it is stored.</summary>
    public string Slug { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public Guid CategoryId { get; private set; }

    public bool IsPublished { get; private set; }

    /// <summary>Lowercase name and slug, searched word by word through a trigram index.</summary>
    public string SearchText { get; private set; } = string.Empty;

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<Product> Create(
        string name, Slug slug, string? description, decimal price, Guid categoryId, DateTimeOffset nowUtc)
    {
        var product = new Product { Id = Guid.CreateVersion7(nowUtc) };
        var applied = product.Apply(name, slug, description, price, categoryId, isPublished: false, nowUtc);
        return applied.IsSuccess ? Result<Product>.Success(product) : Result<Product>.Failure(applied.Error!);
    }

    public Result Update(
        string name, Slug slug, string? description, decimal price, Guid categoryId, bool isPublished, DateTimeOffset nowUtc) =>
        Apply(name, slug, description, price, categoryId, isPublished, nowUtc);

    private Result Apply(
        string name, Slug slug, string? description, decimal price, Guid categoryId, bool isPublished, DateTimeOffset nowUtc)
    {
        var checks = Result.Combine(
            Result.Ensure(!string.IsNullOrWhiteSpace(name) && name.Trim().Length <= NameMaxLength, ProductErrors.InvalidName),
            Result.Ensure(price > 0 && decimal.Round(price, 2) == price, ProductErrors.InvalidPrice),
            Result.Ensure((description?.Length ?? 0) <= DescriptionMaxLength, ProductErrors.DescriptionTooLong));
        if (checks.IsFailure)
            return checks;

        Name = name.Trim();
        Slug = slug.Value;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Price = price;
        CategoryId = categoryId;
        IsPublished = isPublished;
        SearchText = $"{Name} {Slug}".ToLowerInvariant();
        UpdatedAtUtc = nowUtc;
        return Result.Success();
    }
}
