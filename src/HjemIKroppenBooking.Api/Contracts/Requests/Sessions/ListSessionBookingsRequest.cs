namespace HjemIKroppenBooking.Api.Contracts.Requests.Sessions;

public sealed class ListSessionBookingsRequest : PagedRequest
{
    public bool IncludeCancelled { get; init; }
}
