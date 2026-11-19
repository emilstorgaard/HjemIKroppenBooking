using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Features.Users.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.UpdateMyPassword;

public sealed class UpdateMyPasswordCommandHandler(ICurrentUser currentUser, IUserRepository users, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateMyPasswordCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMyPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(currentUser.UserId, cancellationToken) ?? throw DomainException.Unauthorized(ErrorCodes.AuthUnauthorized);
        PasswordCheck.EnsureMatches(passwordHasher, user, command.CurrentPassword, "currentPassword");
        user.ChangePasswordHash(passwordHasher.Hash(command.NewPassword));
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
