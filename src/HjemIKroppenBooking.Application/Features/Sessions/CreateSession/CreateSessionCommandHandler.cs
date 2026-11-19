using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Application.Features.Sessions.CreateSession;

public sealed class CreateSessionCommandHandler(
    ICurrentUser currentUser,
    IServiceTypeRepository serviceTypes,
    ISessionSeriesRepository seriesRepository,
    ISessionRepository sessions,
    ISessionQueries sessionQueries,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CreateSessionCommand, AdminSessionResponse>
{
    public async Task<AdminSessionResponse> Handle(CreateSessionCommand command, CancellationToken cancellationToken)
    {
        var serviceType = await serviceTypes.GetByIdAsync(command.ServiceTypeId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.ServiceTypeNotFound);

        SessionSeries? series = null;
        if (command.SeriesId is { } seriesId)
        {
            series = await seriesRepository.GetByIdAsync(seriesId, cancellationToken) ?? throw DomainException.NotFound(ErrorCodes.SeriesNotFound);
        }

        var duration = TimeSpan.FromMinutes(command.DurationMinutes ?? serviceType.DefaultDurationMinutes);
        var session = Session.Schedule(
            serviceType,
            series,
            command.StartsAt,
            command.StartsAt + duration,
            command.Capacity ?? serviceType.DefaultCapacity,
            command.Location,
            command.Notes,
            currentUser.UserId,
            timeProvider.GetUtcNow().UtcDateTime);

        sessions.Add(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await sessionQueries.GetByIdAsync(session.Id, cancellationToken) ?? throw new InvalidOperationException("The created session could not be read back.");
    }
}
