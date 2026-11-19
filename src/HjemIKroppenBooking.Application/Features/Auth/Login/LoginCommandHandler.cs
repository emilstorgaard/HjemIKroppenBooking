using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Common.Mapping;
using HjemIKroppenBooking.Application.Contracts.Responses.Auth;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Auth.Login;

/// <summary>
/// Timing-safe login: the password is always verified, against a dummy hash when the email is unknown, and unknown
/// email, wrong password and deactivated account all return the same error code.
/// </summary>
public sealed class LoginCommandHandler(IUserRepository users, IPasswordHasher passwordHasher, IJwtTokenService tokenService)
    : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await users.GetByEmailAsync(User.NormalizeEmail(command.Email), cancellationToken);
        var passwordValid = passwordHasher.Verify(command.Password, user?.PasswordHash ?? passwordHasher.DummyHash);

        if (user is null || !passwordValid || !user.IsActive)
        {
            throw DomainException.Unauthorized(ErrorCodes.AuthInvalidCredentials);
        }

        var token = tokenService.Create(user);
        return new LoginResponse(token.Token, token.ExpiresAtUtc, user.ToResponse());
    }
}
