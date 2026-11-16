using BuildingBlocks.Domain.Modeling;
using BuildingBlocks.Domain.Results;

namespace Catalog.Domain.Categories;

/// <summary>A group of products shown as one page of the store.</summary>
public sealed class Category : Entity<Guid>, IAggregateRoot
{
    public const int NameMaxLength = 100;

    private Category()
    {
    }

    public string Name { get; private set; } = string.Empty;

    /// <summary>Checked by <see cref="Domain.Slug"/> before it is stored.</summary>
    public string Slug { get; private set; } = string.Empty;

    public static Result<Category> Create(string name, Slug slug)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > NameMaxLength)
            return CategoryErrors.InvalidName;

        return new Category { Id = Guid.CreateVersion7(), Name = name.Trim(), Slug = slug.Value };
    }
}
