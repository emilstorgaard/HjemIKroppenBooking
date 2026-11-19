using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Sessions.UpdateSession;

/// <summary>
/// Goes through the session aggregate like a booking does, so the row version detects a concurrent booking and the
/// dispatcher retries with fresh data instead of losing an update or overbooking.
/// </summary>
public sealed class UpdateSessionCommandHandler(ISessionRepository sessions, ISessionQueries sessionQueries, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : ICommandHandler<UpdateSessionCommand, AdminSessionResponse>
{
    public async Task<AdminSessionResponse> Handle(UpdateSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await sessions.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SessionNotFound);
        session.Update(command.StartsAt, command.EndsAt, command.Capacity, command.Location, command.Notes, timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await sessionQueries.GetByIdAsync(session.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SessionNotFound);
    }
}
