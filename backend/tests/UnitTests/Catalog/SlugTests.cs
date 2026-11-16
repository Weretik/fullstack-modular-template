using Catalog.Domain;

namespace UnitTests.Catalog;

public sealed class SlugTests
{
    [Theory]
    [InlineData("notebook-a5")]
    [InlineData("pens")]
    public void Create_AcceptsLowercaseWordsJoinedByHyphens(string value)
    {
        var slug = Slug.Create(value);

        Assert.True(slug.IsSuccess);
        Assert.Equal(value, slug.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Notebook")]
    [InlineData("two  words")]
    [InlineData("-leading")]
    [InlineData("trailing-")]
    [InlineData("double--hyphen")]
    public void Create_RejectsAnythingElse(string value)
    {
        var slug = Slug.Create(value);

        Assert.True(slug.IsFailure);
        Assert.Equal(CatalogErrors.InvalidSlug.Code, slug.Error!.Code);
    }
}
