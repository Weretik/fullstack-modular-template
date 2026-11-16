using BuildingBlocks.Domain.Results;
using Mediator;

namespace Catalog.Application.Features.Categories.Queries.ListCategories;

public sealed record ListCategoriesQuery : IQuery<Result<IReadOnlyList<CategoryResponse>>>;
