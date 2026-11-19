namespace HjemIKroppenBooking.Api.Contracts.Requests.Series;

public sealed class ListSeriesParticipantsRequest : PagedRequest
{
    public bool IncludeCancelled { get; init; }
}
