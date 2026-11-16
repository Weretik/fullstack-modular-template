using System.Text.RegularExpressions;
using BuildingBlocks.Domain.Results;

namespace Catalog.Domain;

/// <summary>The URL part of a category or product page: lowercase latin words and digits joined by single hyphens.</summary>
public sealed partial record Slug
{
    public const int MaxLength = 120;

    private Slug(string value) => Value = value;

    public string Value { get; }

    public static Result<Slug> Create(string? value) =>
        value is { Length: > 0 and <= MaxLength } && Pattern().IsMatch(value)
            ? new Slug(value)
            : CatalogErrors.InvalidSlug;

    public override string ToString() => Value;

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex Pattern();
}
