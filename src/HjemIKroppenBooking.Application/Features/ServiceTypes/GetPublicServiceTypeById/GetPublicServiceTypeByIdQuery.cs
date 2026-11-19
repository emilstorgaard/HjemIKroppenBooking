using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.ServiceTypes.GetPublicServiceTypeById;

public sealed record GetPublicServiceTypeByIdQuery(Guid Id) : IQuery<ServiceTypeResponse>;
