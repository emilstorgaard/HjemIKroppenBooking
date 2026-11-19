using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Application.Features.Series.CreateSeries;

public sealed class CreateSeriesCommandHandler(
    ICurrentUser currentUser,
    IServiceTypeRepository serviceTypes,
    ISessionSeriesRepository seriesRepository,
    ISessionRepository sessions,
    ISeriesQueries seriesQueries,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CreateSeriesCommand, AdminSeriesResponse>
{
    public async Task<AdminSeriesResponse> Handle(CreateSeriesCommand command, CancellationToken cancellationToken)
    {
        var serviceType = await serviceTypes.GetByIdAsync(command.ServiceTypeId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceTypeNotFound);
        var series = SessionSeries.Create(serviceType, command.Title);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var duration = TimeSpan.FromMinutes(command.DurationMinutes ?? serviceType.DefaultDurationMinutes);
        var capacity = command.Capacity ?? serviceType.DefaultCapacity;
        foreach (var start in command.StartTimes.Order())
        {
            sessions.Add(Session.Schedule(serviceType, series, start, start + duration, capacity, command.Location, command.Notes, currentUser.UserId, now));
        }

        // One save, so the series and all its sessions are created together or not at all.
        seriesRepository.Add(series);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await seriesQueries.GetByIdAsync(series.Id, cancellationToken) ?? throw new InvalidOperationException("The created series could not be read back.");
    }
}
