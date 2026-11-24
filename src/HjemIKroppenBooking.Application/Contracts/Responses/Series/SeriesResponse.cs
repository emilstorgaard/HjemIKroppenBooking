using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Series;

/// <summary>
/// A series as shown to users. <see cref="IsBookable"/> is true while at least one session has not started.
/// <see cref="AvailableSpots"/> is the lowest number of free spots among the sessions that have not started,
/// because booking a series books all of them.
/// </summary>
public sealed record SeriesResponse(
    Guid Id,
    string Title,
    Guid ServiceTypeId,
    string ServiceTypeName,
    ServiceCategorySummary Category,
    bool IsBookable,
    int AvailableSpots,
    DateTime? FirstStartsAt,
    DateTime? LastEndsAt,
    IReadOnlyList<SeriesSessionResponse> Sessions);
