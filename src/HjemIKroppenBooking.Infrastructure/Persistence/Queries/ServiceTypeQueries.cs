using System.Linq.Expressions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.ListPublicServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.ListServiceTypes;
using HjemIKroppenBooking.Domain.ServiceTypes;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Queries;

internal sealed class ServiceTypeQueries(AppDbContext db) : IServiceTypeQueries
{
    private static readonly Expression<Func<ServiceType, ServiceTypeResponse>> ToPublic =
        s => new ServiceTypeResponse(s.Id, s.Category, s.Name, s.Description, s.DefaultDurationMinutes, s.DefaultCapacity, s.SortOrder);

    private static readonly Expression<Func<ServiceType, AdminServiceTypeResponse>> ToAdmin =
        s => new AdminServiceTypeResponse(
            s.Id, s.Category, s.Name, s.Description, s.DefaultDurationMinutes, s.DefaultCapacity, s.SortOrder, s.IsActive, s.CreatedAtUtc, s.UpdatedAtUtc);

    public Task<PagedResponse<ServiceTypeResponse>> ListPublicAsync(ListPublicServiceTypesQuery query, CancellationToken cancellationToken)
    {
        var serviceTypes = db.ServiceTypes.AsNoTracking().Where(s => s.IsActive);
        if (query.Category is { } category)
        {
            serviceTypes = serviceTypes.Where(s => s.Category == category);
        }

        return serviceTypes.ToPagedResponseAsync(Order, ToPublic, query.Page, query.PageSize, cancellationToken);
    }

    public Task<ServiceTypeResponse?> GetPublicByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.ServiceTypes.AsNoTracking().Where(s => s.Id == id && s.IsActive).Select(ToPublic).SingleOrDefaultAsync(cancellationToken);

    public Task<PagedResponse<AdminServiceTypeResponse>> ListAsync(ListServiceTypesQuery query, CancellationToken cancellationToken)
    {
        var serviceTypes = db.ServiceTypes.AsNoTracking();
        if (query.Category is { } category)
        {
            serviceTypes = serviceTypes.Where(s => s.Category == category);
        }

        if (query.IsActive is { } isActive)
        {
            serviceTypes = serviceTypes.Where(s => s.IsActive == isActive);
        }

        return serviceTypes.ToPagedResponseAsync(Order, ToAdmin, query.Page, query.PageSize, cancellationToken);
    }

    public Task<AdminServiceTypeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.ServiceTypes.AsNoTracking().Where(s => s.Id == id).Select(ToAdmin).SingleOrDefaultAsync(cancellationToken);

    private static IOrderedQueryable<ServiceType> Order(IQueryable<ServiceType> query) =>
        query.OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ThenBy(s => s.Id);
}
