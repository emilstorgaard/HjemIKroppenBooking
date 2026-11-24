namespace HjemIKroppenBooking.Api.Contracts.Requests.Sessions;

public sealed class ListPublicSessionsRequest : PagedRequest
{
    public Guid? ServiceTypeId { get; init; }

    public Guid? CategoryId { get; init; }

    /// <summary>Session starts at or after this time (inclusive).</summary>
    public DateTimeOffset? From { get; init; }

    /// <summary>Session starts before this time (exclusive).</summary>
    public DateTimeOffset? To { get; init; }

    /// <summary>Only sessions with at least one free spot.</summary>
    public bool OnlyAvailable { get; init; }
}
