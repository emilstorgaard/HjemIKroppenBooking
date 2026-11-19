using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Sessions.GetSessionById;

public sealed class GetSessionByIdQueryHandler(ISessionQueries sessions) : IQueryHandler<GetSessionByIdQuery, AdminSessionResponse>
{
    public async Task<AdminSessionResponse> Handle(GetSessionByIdQuery query, CancellationToken cancellationToken) =>
        await sessions.GetByIdAsync(query.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SessionNotFound);
}
