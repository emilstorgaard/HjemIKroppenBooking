namespace HjemIKroppenBooking.Api.Contracts.Requests.Sessions;

/// <summary>Duration and capacity default to the service type's values when omitted.</summary>
public sealed record CreateSessionRequest(
    Guid ServiceTypeId, Guid? SeriesId, DateTimeOffset StartsAt, int? DurationMinutes, int? Capacity, string? Location, string? Notes);
