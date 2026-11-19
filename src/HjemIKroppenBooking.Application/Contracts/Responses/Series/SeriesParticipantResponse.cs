namespace HjemIKroppenBooking.Application.Contracts.Responses.Series;

/// <summary>One row per user with bookings in the series.</summary>
public sealed record SeriesParticipantResponse(Guid UserId, string FullName, string Email, int ActiveBookingCount, string? Note, DateTime FirstBookedAt);
