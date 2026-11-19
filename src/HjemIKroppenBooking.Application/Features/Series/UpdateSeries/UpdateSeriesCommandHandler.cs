using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Series.UpdateSeries;

public sealed class UpdateSeriesCommandHandler(ISessionSeriesRepository seriesRepository, ISeriesQueries seriesQueries, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateSeriesCommand, AdminSeriesResponse>
{
    public async Task<AdminSeriesResponse> Handle(UpdateSeriesCommand command, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByIdAsync(command.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
        series.Rename(command.Title);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await seriesQueries.GetByIdAsync(series.Id, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
    }
}
