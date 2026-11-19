using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Sessions.DeleteSession;

/// <summary>
/// Deletes a session without active bookings. Its cancelled bookings are deleted with it. A session with active
/// bookings must be cancelled (or its bookings cancelled) first.
/// </summary>
public sealed record DeleteSessionCommand(Guid Id) : ICommand<Unit>;
