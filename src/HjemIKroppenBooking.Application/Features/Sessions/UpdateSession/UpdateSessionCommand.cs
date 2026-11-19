using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Sessions.UpdateSession;

/// <summary>Full replacement of time, capacity and details. Capacity can never go below the active bookings.</summary>
public sealed record UpdateSessionCommand(
    Guid Id, DateTime StartsAt, DateTime EndsAt, int Capacity, string? Location, string? Notes) : ICommand<AdminSessionResponse>;
