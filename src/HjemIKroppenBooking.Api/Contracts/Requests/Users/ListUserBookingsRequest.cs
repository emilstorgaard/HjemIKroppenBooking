using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Api.Contracts.Requests.Users;

public sealed class ListUserBookingsRequest : PagedRequest
{
    public BookingStatus? Status { get; init; }

    /// <summary>true: the session has not ended. false: it has ended.</summary>
    public bool? Upcoming { get; init; }

    public Guid? SeriesId { get; init; }
}
