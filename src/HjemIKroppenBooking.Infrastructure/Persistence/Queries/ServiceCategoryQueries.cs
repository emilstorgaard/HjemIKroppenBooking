using System.Linq.Expressions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.ListPublicServiceCategories;
using HjemIKroppenBooking.Application.Features.ServiceCategories.ListServiceCategories;
using HjemIKroppenBooking.Domain.ServiceCategories;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Queries;

internal sealed class ServiceCategoryQueries(AppDbContext db) : IServiceCategoryQueries
{
    public Task<PagedResponse<ServiceCategoryResponse>> ListPublicAsync(ListPublicServiceCategoriesQuery query, CancellationToken cancellationToken) =>
        db.ServiceCategories.AsNoTracking()
            .ToPagedResponseAsync(Order, c => new ServiceCategoryResponse(c.Id, c.Name, c.Slug), query.Page, query.PageSize, cancellationToken);

    public Task<PagedResponse<AdminServiceCategoryResponse>> ListAsync(ListServiceCategoriesQuery query, CancellationToken cancellationToken) =>
        db.ServiceCategories.AsNoTracking().ToPagedResponseAsync(Order, ToAdmin(), query.Page, query.PageSize, cancellationToken);

    public Task<AdminServiceCategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.ServiceCategories.AsNoTracking().Where(c => c.Id == id).Select(ToAdmin()).SingleOrDefaultAsync(cancellationToken);

    /// <summary>Built per query because the service type count needs the context's ServiceTypes set.</summary>
    private Expression<Func<ServiceCategory, AdminServiceCategoryResponse>> ToAdmin() =>
        c => new AdminServiceCategoryResponse(
            c.Id, c.Name, c.Slug, db.ServiceTypes.Count(s => s.CategoryId == c.Id), c.CreatedAtUtc, c.UpdatedAtUtc);

    private static IOrderedQueryable<ServiceCategory> Order(IQueryable<ServiceCategory> query) =>
        query.OrderBy(c => c.CreatedAtUtc).ThenBy(c => c.Name).ThenBy(c => c.Id);
}
