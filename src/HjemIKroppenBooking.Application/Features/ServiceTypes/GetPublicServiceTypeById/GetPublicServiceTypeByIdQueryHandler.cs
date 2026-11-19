using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.GetPublicServiceTypeById;

/// <summary>Inactive service types give the same response as missing ones.</summary>
public sealed class GetPublicServiceTypeByIdQueryHandler(IServiceTypeQueries serviceTypes) : IQueryHandler<GetPublicServiceTypeByIdQuery, ServiceTypeResponse>
{
    public async Task<ServiceTypeResponse> Handle(GetPublicServiceTypeByIdQuery query, CancellationToken cancellationToken) =>
        await serviceTypes.GetPublicByIdAsync(query.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceTypeNotFound);
}
