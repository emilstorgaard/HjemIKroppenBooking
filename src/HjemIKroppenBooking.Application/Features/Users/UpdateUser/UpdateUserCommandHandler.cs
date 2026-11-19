using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Common.Mapping;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Features.Users.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.UpdateUser;

public sealed class UpdateUserCommandHandler(IUserRepository users, IUnitOfWork unitOfWork) : ICommandHandler<UpdateUserCommand, AdminUserResponse>
{
    public async Task<AdminUserResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.UserNotFound);
        await UserProfileUpdater.UpdateAsync(users, user, command.FullName, command.Email, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToAdminResponse();
    }
}
