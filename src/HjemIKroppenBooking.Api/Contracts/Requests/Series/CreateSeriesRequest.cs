namespace HjemIKroppenBooking.Api.Contracts.Requests.Series;

/// <summary>One session per start time. Duration and capacity default to the service type's values when omitted.</summary>
public sealed record CreateSeriesRequest(
    Guid ServiceTypeId, string? Title, IReadOnlyList<DateTimeOffset> StartTimes, int? DurationMinutes, int? Capacity, string? Location, string? Notes);
