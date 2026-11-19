using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Sessions.DeleteSession;

public sealed class DeleteSessionCommandHandler(ISessionRepository sessions, IUnitOfWork unitOfWork) : ICommandHandler<DeleteSessionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await sessions.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SessionNotFound);
        session.EnsureCanBeDeleted();
        sessions.Remove(session);

        // The row version makes the delete fail if a booking was added after the session was loaded.
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
