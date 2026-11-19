using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Api.Contracts.Requests.Bookings;

public sealed class ListBookingsRequest : PagedRequest
{
    public Guid? UserId { get; init; }

    public Guid? SessionId { get; init; }

    public Guid? SeriesId { get; init; }

    public Guid? ServiceTypeId { get; init; }

    public BookingStatus? Status { get; init; }

    /// <summary>true: the session has not ended. false: it has ended.</summary>
    public bool? Upcoming { get; init; }

    /// <summary>Session starts at or after this time (inclusive).</summary>
    public DateTimeOffset? From { get; init; }

    /// <summary>Session starts before this time (exclusive).</summary>
    public DateTimeOffset? To { get; init; }
}
