using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.ResetUserPassword;

public sealed record ResetUserPasswordCommand(Guid Id, string NewPassword) : ICommand<Unit>;
