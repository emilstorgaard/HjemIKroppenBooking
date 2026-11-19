using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Features.Bookings.Common;

/// <summary>
/// Filters for booking lists. <see cref="Upcoming"/>: true = session not ended, false = ended, null = all.
/// <see cref="FromUtc"/> is inclusive and <see cref="ToUtc"/> exclusive, both on the session start.
/// Sorting: ascending session start for upcoming lists, otherwise descending; then booking time, then id.
/// </summary>
public sealed record BookingListFilter(
    DateTime NowUtc,
    Guid? UserId = null,
    Guid? SessionId = null,
    Guid? SeriesId = null,
    Guid? ServiceTypeId = null,
    BookingStatus? Status = null,
    bool? Upcoming = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null);
