using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Features.Series.ListBookableSeries;
using HjemIKroppenBooking.Application.Features.Series.ListSeries;
using HjemIKroppenBooking.Application.Features.Series.ListSeriesParticipants;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Read side for series.</summary>
public interface ISeriesQueries
{
    /// <summary>Non-cancelled series of active service types with at least one session that has not started.</summary>
    Task<PagedResponse<SeriesResponse>> ListBookableAsync(ListBookableSeriesQuery query, DateTime nowUtc, CancellationToken cancellationToken);

    /// <summary>A non-cancelled series of an active service type, also when it has started or ended.</summary>
    Task<SeriesResponse?> GetPublicByIdAsync(Guid id, DateTime nowUtc, CancellationToken cancellationToken);

    Task<PagedResponse<AdminSeriesListItemResponse>> ListAsync(ListSeriesQuery query, DateTime nowUtc, CancellationToken cancellationToken);

    Task<AdminSeriesResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResponse<SeriesParticipantResponse>> ListParticipantsAsync(ListSeriesParticipantsQuery query, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
