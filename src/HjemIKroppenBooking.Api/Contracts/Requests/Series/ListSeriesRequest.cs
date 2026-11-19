namespace HjemIKroppenBooking.Api.Contracts.Requests.Series;

public sealed class ListSeriesRequest : PagedRequest
{
    /// <summary>Matches the title or the service type name.</summary>
    public string? Search { get; init; }

    public Guid? ServiceTypeId { get; init; }

    /// <summary>true: has a non-cancelled session that has not ended.</summary>
    public bool? Upcoming { get; init; }

    public bool? IsCancelled { get; init; }
}
