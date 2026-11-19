using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Series;

/// <summary>The bookings created or cancelled by an admin series booking operation for a user.</summary>
public sealed record AdminSeriesBookingResponse(Guid SeriesId, Guid UserId, IReadOnlyList<AdminBookingResponse> Bookings);
