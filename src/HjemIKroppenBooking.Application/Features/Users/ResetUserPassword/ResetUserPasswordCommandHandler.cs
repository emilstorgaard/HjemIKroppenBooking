using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Features.Users.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.ResetUserPassword;

public sealed class ResetUserPasswordCommandHandler(ICurrentUser currentUser, IUserRepository users, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    : ICommandHandler<ResetUserPasswordCommand, Unit>
{
    public async Task<Unit> Handle(ResetUserPasswordCommand command, CancellationToken cancellationToken)
    {
        OwnAccountGuard.EnsureNotSelf(currentUser, command.Id);
        var user = await users.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.UserNotFound);
        user.ChangePasswordHash(passwordHasher.Hash(command.NewPassword));
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
