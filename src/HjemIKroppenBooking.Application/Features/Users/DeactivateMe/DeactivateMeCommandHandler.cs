using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Features.Users.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.DeactivateMe;

public sealed class DeactivateMeCommandHandler(ICurrentUser currentUser, IUserRepository users, IPasswordHasher passwordHasher, UserDeactivator deactivator)
    : ICommandHandler<DeactivateMeCommand, Unit>
{
    public async Task<Unit> Handle(DeactivateMeCommand command, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(currentUser.UserId, cancellationToken) ?? throw DomainException.Unauthorized(ErrorCodes.AuthUnauthorized);
        PasswordCheck.EnsureMatches(passwordHasher, user, command.Password, "password");
        await deactivator.DeactivateAsync(user, cancellationToken);
        return Unit.Value;
    }
}
