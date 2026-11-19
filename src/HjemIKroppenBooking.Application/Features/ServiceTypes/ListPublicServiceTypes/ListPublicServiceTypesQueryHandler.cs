using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ListPublicServiceTypes;

public sealed class ListPublicServiceTypesQueryHandler(IServiceTypeQueries serviceTypes)
    : IQueryHandler<ListPublicServiceTypesQuery, PagedResponse<ServiceTypeResponse>>
{
    public Task<PagedResponse<ServiceTypeResponse>> Handle(ListPublicServiceTypesQuery query, CancellationToken cancellationToken) =>
        serviceTypes.ListPublicAsync(query, cancellationToken);
}
