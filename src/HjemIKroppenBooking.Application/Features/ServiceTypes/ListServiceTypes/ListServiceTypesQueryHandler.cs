using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.ListServiceTypes;

public sealed class ListServiceTypesQueryHandler(IServiceTypeQueries serviceTypes)
    : IQueryHandler<ListServiceTypesQuery, PagedResponse<AdminServiceTypeResponse>>
{
    public Task<PagedResponse<AdminServiceTypeResponse>> Handle(ListServiceTypesQuery query, CancellationToken cancellationToken) =>
        serviceTypes.ListAsync(query, cancellationToken);
}
