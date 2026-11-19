using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Users.ChangeUserRole;

/// <summary>The new role takes effect at the user's next login, when a new token is issued.</summary>
public sealed record ChangeUserRoleCommand(Guid Id, UserRole Role) : ICommand<AdminUserResponse>;
