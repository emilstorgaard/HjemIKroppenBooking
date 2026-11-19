using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.ActivateUser;

public sealed record ActivateUserCommand(Guid Id) : ICommand<Unit>;
