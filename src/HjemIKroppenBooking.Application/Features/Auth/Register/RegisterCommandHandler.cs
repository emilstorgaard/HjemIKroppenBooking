using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Common.Mapping;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Auth.Register;

/// <summary>Registers a user with the User role. The role is never taken from the request.</summary>
public sealed class RegisterCommandHandler(IUserRepository users, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    : ICommandHandler<RegisterCommand, UserResponse>
{
    public async Task<UserResponse> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var email = User.NormalizeEmail(command.Email);
        if (await users.EmailExistsAsync(email, null, cancellationToken))
        {
            throw DomainException.Conflict(ErrorCodes.UserEmailAlreadyExists);
        }

        var user = User.Register(email, command.FullName, passwordHasher.Hash(command.Password));
        users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToResponse();
    }
}
