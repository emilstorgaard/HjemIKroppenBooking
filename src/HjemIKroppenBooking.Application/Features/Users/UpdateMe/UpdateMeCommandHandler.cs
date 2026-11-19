using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Common.Mapping;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Features.Users.Common;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Users.UpdateMe;

public sealed class UpdateMeCommandHandler(ICurrentUser currentUser, IUserRepository users, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateMeCommand, UserResponse>
{
    public async Task<UserResponse> Handle(UpdateMeCommand command, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(currentUser.UserId, cancellationToken) ?? throw DomainException.Unauthorized(ErrorCodes.AuthUnauthorized);
        await UserProfileUpdater.UpdateAsync(users, user, command.FullName, command.Email, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToResponse();
    }
}
