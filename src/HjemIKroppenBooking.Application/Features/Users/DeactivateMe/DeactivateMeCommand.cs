using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.DeactivateMe;

/// <summary>Deactivates the signed-in user's own account. The password is required as confirmation.</summary>
public sealed record DeactivateMeCommand(string Password) : ICommand<Unit>;
