using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Series.CancelSeries;

public sealed class CancelSeriesCommandHandler(
    ISessionSeriesRepository seriesRepository, ISessionRepository sessions, ISeriesQueries seriesQueries, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : ICommandHandler<CancelSeriesCommand, AdminSeriesResponse>
{
    public async Task<AdminSeriesResponse> Handle(CancelSeriesCommand command, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
        var seriesSessions = await sessions.GetBySeriesAsync(series.Id, cancellationToken);
        series.Cancel(seriesSessions, timeProvider.GetUtcNow().UtcDateTime);

        // One save: every changed session row is checked against its row version in the same transaction.
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await seriesQueries.GetByIdAsync(series.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
    }
}
