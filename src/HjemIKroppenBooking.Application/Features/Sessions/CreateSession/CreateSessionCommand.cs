using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Sessions.CreateSession;

/// <summary>
/// Creates a session, optionally in an existing series. Duration and capacity default to the service type's values.
/// Overlaps with other sessions are allowed.
/// </summary>
public sealed record CreateSessionCommand(
    Guid ServiceTypeId,
    Guid? SeriesId,
    DateTime StartsAt,
    int? DurationMinutes,
    int? Capacity,
    string? Location,
    string? Notes) : ICommand<AdminSessionResponse>;
