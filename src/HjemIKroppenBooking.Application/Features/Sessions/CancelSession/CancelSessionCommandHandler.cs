using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Sessions.CancelSession;

public sealed class CancelSessionCommandHandler(ISessionRepository sessions, ISessionQueries sessionQueries, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : ICommandHandler<CancelSessionCommand, AdminSessionResponse>
{
    public async Task<AdminSessionResponse> Handle(CancelSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await sessions.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SessionNotFound);
        session.Cancel(timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await sessionQueries.GetByIdAsync(session.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SessionNotFound);
    }
}
