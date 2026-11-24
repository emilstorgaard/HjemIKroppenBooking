namespace HjemIKroppenBooking.Application.Features.Sessions.Common;

/// <summary>
/// Filters for session lists. <see cref="Upcoming"/>: true = not ended, false = ended, null = all.
/// <see cref="FromUtc"/> is inclusive and <see cref="ToUtc"/> exclusive, both on the start time.
/// Sorting: ascending start for upcoming lists, otherwise descending start; then id.
/// </summary>
public sealed record SessionListFilter(
    DateTime NowUtc,
    Guid? ServiceTypeId = null,
    Guid? CategoryId = null,
    Guid? SeriesId = null,
    bool? IsCancelled = null,
    bool? Upcoming = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    bool OnlyAvailable = false
);
