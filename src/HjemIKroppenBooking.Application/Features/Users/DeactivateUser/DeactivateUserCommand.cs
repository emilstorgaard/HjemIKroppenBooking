using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.DeactivateUser;

public sealed record DeactivateUserCommand(Guid Id) : ICommand<Unit>;
