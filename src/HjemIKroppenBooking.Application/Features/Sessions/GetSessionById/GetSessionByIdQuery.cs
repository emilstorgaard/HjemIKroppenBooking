using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Sessions.GetSessionById;

public sealed record GetSessionByIdQuery(Guid Id) : IQuery<AdminSessionResponse>;
