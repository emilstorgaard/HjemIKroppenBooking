using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.UpdateUser;

public sealed record UpdateUserCommand(Guid Id, string FullName, string Email) : ICommand<AdminUserResponse>;
