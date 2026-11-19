using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Sessions.GetPublicSessionById;

/// <summary>Cancelled sessions and sessions of inactive service types give the same response as missing ones.</summary>
public sealed class GetPublicSessionByIdQueryHandler(ISessionQueries sessions) : IQueryHandler<GetPublicSessionByIdQuery, SessionResponse>
{
    public async Task<SessionResponse> Handle(GetPublicSessionByIdQuery query, CancellationToken cancellationToken) =>
        await sessions.GetPublicByIdAsync(query.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SessionNotFound);
}
