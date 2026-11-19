using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.GetServiceTypeById;

public sealed class GetServiceTypeByIdQueryHandler(IServiceTypeQueries serviceTypes) : IQueryHandler<GetServiceTypeByIdQuery, AdminServiceTypeResponse>
{
    public async Task<AdminServiceTypeResponse> Handle(GetServiceTypeByIdQuery query, CancellationToken cancellationToken) =>
        await serviceTypes.GetByIdAsync(query.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceTypeNotFound);
}
