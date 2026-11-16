using BuildingBlocks.Domain.Results;
using Catalog.Application.Features.Categories.Queries.ListCategories;
using Mediator;

namespace Catalog.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(string Name, string Slug) : ICommand<Result<CategoryResponse>>;
