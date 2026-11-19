using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Series;

/// <summary>The bookings created or cancelled by a series booking operation of the signed-in user.</summary>
public sealed record SeriesBookingResponse(Guid SeriesId, IReadOnlyList<BookingResponse> Bookings);
