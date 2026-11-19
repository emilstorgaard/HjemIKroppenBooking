using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Abstractions;

public interface IJwtTokenService
{
    AccessToken Create(User user);
}
