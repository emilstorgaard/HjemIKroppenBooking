using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Series.CreateSeries;

/// <summary>
/// Creates a series of a group service type with one session per start time. Duration and capacity default to the
/// service type's values. Start times may overlap other sessions and series.
/// </summary>
public sealed record CreateSeriesCommand(
    Guid ServiceTypeId,
    string? Title,
    IReadOnlyList<DateTime> StartTimes,
    int? DurationMinutes,
    int? Capacity,
    string? Location,
    string? Notes) : ICommand<AdminSeriesResponse>;
