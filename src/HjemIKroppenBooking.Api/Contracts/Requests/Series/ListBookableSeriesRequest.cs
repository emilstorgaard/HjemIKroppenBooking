namespace HjemIKroppenBooking.Api.Contracts.Requests.Series;

public sealed class ListBookableSeriesRequest : PagedRequest
{
    public Guid? ServiceTypeId { get; init; }

    /// <summary>Only series where none of the remaining sessions is full.</summary>
    public bool OnlyAvailable { get; init; }
}
