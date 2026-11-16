using Catalog.Domain;
using Catalog.Domain.Products;

namespace UnitTests.Catalog;

public sealed class ProductTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 10, 9, 0, 0, TimeSpan.Zero);
    private static readonly Guid CategoryId = Guid.NewGuid();

    [Fact]
    public void Create_StartsAsDraft_AndBuildsSearchTextFromNameAndSlug()
    {
        var product = Product.Create("Notebook A5", Slug.Create("notebook-a5").Value, "Dotted pages", 120.50m, CategoryId, Now);

        Assert.True(product.IsSuccess);
        Assert.False(product.Value.IsPublished);
        Assert.Equal("notebook a5 notebook-a5", product.Value.SearchText);
        Assert.Equal(Now, product.Value.UpdatedAtUtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10.555)]
    public void Create_RejectsAPriceThatIsNotPositiveMoney(decimal price)
    {
        var product = Product.Create("Pen", Slug.Create("pen").Value, null, price, CategoryId, Now);

        Assert.Equal(ProductErrors.InvalidPrice, product.Error);
    }

    [Fact]
    public void Update_ChangesDataAndPublication()
    {
        var product = Product.Create("Pen", Slug.Create("pen").Value, null, 10m, CategoryId, Now).Value;
        var later = Now.AddHours(1);

        var updated = product.Update("Blue pen", Slug.Create("blue-pen").Value, "Gel", 12m, CategoryId, isPublished: true, later);

        Assert.True(updated.IsSuccess);
        Assert.Equal("Blue pen", product.Name);
        Assert.True(product.IsPublished);
        Assert.Equal(later, product.UpdatedAtUtc);
    }
}
