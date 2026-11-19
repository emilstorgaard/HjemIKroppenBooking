using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Features.Users.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.DeactivateUser;

public sealed class DeactivateUserCommandHandler(ICurrentUser currentUser, IUserRepository users, UserDeactivator deactivator)
    : ICommandHandler<DeactivateUserCommand, Unit>
{
    public async Task<Unit> Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
    {
        OwnAccountGuard.EnsureNotSelf(currentUser, command.Id);
        var user = await users.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.UserNotFound);
        await deactivator.DeactivateAsync(user, cancellationToken);
        return Unit.Value;
    }
}
