using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Api.Contracts.Requests.Users;

public sealed record ChangeUserRoleRequest(UserRole Role);
