using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.UpdateMyPassword;

public sealed record UpdateMyPasswordCommand(string CurrentPassword, string NewPassword) : ICommand<Unit>;
