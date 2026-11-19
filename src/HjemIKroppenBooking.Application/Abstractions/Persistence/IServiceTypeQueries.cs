using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.ListPublicServiceTypes;
using HjemIKroppenBooking.Application.Features.ServiceTypes.ListServiceTypes;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Read side for service types. Public methods only return active service types.</summary>
public interface IServiceTypeQueries
{
    Task<PagedResponse<ServiceTypeResponse>> ListPublicAsync(ListPublicServiceTypesQuery query, CancellationToken cancellationToken);

    Task<ServiceTypeResponse?> GetPublicByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResponse<AdminServiceTypeResponse>> ListAsync(ListServiceTypesQuery query, CancellationToken cancellationToken);

    Task<AdminServiceTypeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
