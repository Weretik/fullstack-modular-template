using Catalog.Application.Features.Products.Commands.CreateProduct;
using Catalog.Domain;
using Catalog.Domain.Categories;
using Catalog.Domain.Products;
using Microsoft.Extensions.Time.Testing;
using UnitTests.TestDoubles;

namespace UnitTests.Catalog;

public sealed class CreateProductHandlerTests
{
    private readonly TestCatalogDbContext _db = new();
    private readonly CreateProductHandler _handler;
    private readonly Category _pens = Category.Create("Pens", Slug.Create("pens").Value).Value;

    public CreateProductHandlerTests()
    {
        _db.Categories.Add(_pens);
        _db.SaveChanges();
        _handler = new CreateProductHandler(_db, new FakeTimeProvider(DateTimeOffset.UtcNow));
    }

    [Fact]
    public async Task Creates_ADraftProduct()
    {
        var result = await _handler.Handle(new CreateProductCommand("Gel pen", "gel-pen", null, 39m, _pens.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsPublished);
        Assert.Single(_db.Products);
    }

    [Fact]
    public async Task A_SlugInUse_IsAConflict_SoADoubleSubmitCannotCreateTwo()
    {
        await _handler.Handle(new CreateProductCommand("Gel pen", "gel-pen", null, 39m, _pens.Id), CancellationToken.None);
        var repeat = await _handler.Handle(new CreateProductCommand("Gel pen", "gel-pen", null, 39m, _pens.Id), CancellationToken.None);

        Assert.Equal(CatalogErrors.SlugTaken, repeat.Error);
        Assert.Single(_db.Products);
    }

    [Fact]
    public async Task An_UnknownCategory_IsRefused()
    {
        var result = await _handler.Handle(new CreateProductCommand("Gel pen", "gel-pen", null, 39m, Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(ProductErrors.UnknownCategory, result.Error);
    }
}
