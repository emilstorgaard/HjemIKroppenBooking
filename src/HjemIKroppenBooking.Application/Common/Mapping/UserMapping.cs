using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Common.Mapping;

public static class UserMapping
{
    public static UserResponse ToResponse(this User user) => new(user.Id, user.Email, user.FullName, user.Role);

    public static AdminUserResponse ToAdminResponse(this User user) =>
        new(user.Id, user.Email, user.FullName, user.Role, user.IsActive, user.CreatedAtUtc, user.UpdatedAtUtc);
}
