using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.ActivateUser;

public sealed class ActivateUserCommandHandler(IUserRepository users, IUnitOfWork unitOfWork) : ICommandHandler<ActivateUserCommand, Unit>
{
    public async Task<Unit> Handle(ActivateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.UserNotFound);
        user.Activate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
