namespace HjemIKroppenBooking.Application.Contracts.Responses.Series;

/// <summary>A non-cancelled session in a series as shown to users.</summary>
public sealed record SeriesSessionResponse(
    Guid Id, DateTime StartsAt, DateTime EndsAt, string? Location, string? Notes, int Capacity, int AvailableSpots, bool HasStarted);
