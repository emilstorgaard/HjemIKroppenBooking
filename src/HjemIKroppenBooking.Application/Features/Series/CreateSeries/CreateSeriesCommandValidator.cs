using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Application.Features.Series.CreateSeries;

public sealed class CreateSeriesCommandValidator : AppValidator<CreateSeriesCommand>
{
    public CreateSeriesCommandValidator()
    {
        RuleFor(x => x.ServiceTypeId).RequiredId();
        RuleFor(x => x.Title).OptionalText(SessionSeries.TitleMaxLength);
        RuleFor(x => x.StartTimes)
            .NotEmpty().WithErrorCode(ErrorCodes.ValidationRequired)
            .Must(starts => starts.Count <= SessionSeries.MaxSessions).WithErrorCode(ErrorCodes.ValidationTooManyItems);
        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(Session.MinDurationMinutes, Session.MaxDurationMinutes).WithErrorCode(ErrorCodes.ValidationDurationOutOfRange)
            .When(x => x.DurationMinutes is not null);
        RuleFor(x => x.Capacity)
            .InclusiveBetween(Session.MinCapacity, Session.MaxCapacity).WithErrorCode(ErrorCodes.ValidationCapacityOutOfRange)
            .When(x => x.Capacity is not null);
        RuleFor(x => x.Location).OptionalText(Session.LocationMaxLength);
        RuleFor(x => x.Notes).OptionalText(Session.NotesMaxLength);
    }
}
