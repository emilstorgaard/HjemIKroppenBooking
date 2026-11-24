namespace HjemIKroppenBooking.Api.Contracts.Requests.Sessions;

public sealed class ListSessionsRequest : PagedRequest
{
    public Guid? ServiceTypeId { get; init; }

    public Guid? CategoryId { get; init; }

    public Guid? SeriesId { get; init; }

    public bool? IsCancelled { get; init; }

    /// <summary>true: not ended. false: ended.</summary>
    public bool? Upcoming { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public bool OnlyAvailable { get; init; }
}
