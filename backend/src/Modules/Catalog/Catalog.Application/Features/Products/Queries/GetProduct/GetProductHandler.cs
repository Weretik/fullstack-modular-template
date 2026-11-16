using BuildingBlocks.Domain.Results;
using Catalog.Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Products.Queries.GetProduct;

public sealed class GetProductHandler(ICatalogDbContext db) : IQueryHandler<GetProductQuery, Result<ProductResponse>>
{
    public async ValueTask<Result<ProductResponse>> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await db.Products
            .AsNoTracking()
            .Where(p => p.Id == query.Id)
            .Select(ProductMapping.ToResponse)
            .FirstOrDefaultAsync(cancellationToken);
        return product is null ? ProductErrors.NotFound : product;
    }
}
