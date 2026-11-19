using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Common.Mapping;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Features.Users.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.ChangeUserRole;

public sealed class ChangeUserRoleCommandHandler(ICurrentUser currentUser, IUserRepository users, IUnitOfWork unitOfWork)
    : ICommandHandler<ChangeUserRoleCommand, AdminUserResponse>
{
    public async Task<AdminUserResponse> Handle(ChangeUserRoleCommand command, CancellationToken cancellationToken)
    {
        OwnAccountGuard.EnsureNotSelf(currentUser, command.Id);
        var user = await users.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.UserNotFound);
        user.ChangeRole(command.Role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToAdminResponse();
    }
}
