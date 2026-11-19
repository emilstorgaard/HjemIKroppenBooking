using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.UnitTests.Application.Fakes;

internal sealed class FakeJwtTokenService(DateTime expiresAtUtc) : IJwtTokenService
{
    public AccessToken Create(User user) => new("token-for-" + user.Email, expiresAtUtc);
}
