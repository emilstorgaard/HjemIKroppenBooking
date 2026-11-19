using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Application.Features.Sessions.CreateSession;

public sealed class CreateSessionCommandValidator : AppValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.ServiceTypeId).RequiredId();
        RuleFor(x => x.SeriesId).NotEqual(Guid.Empty).WithErrorCode(ErrorCodes.ValidationInvalidValue);
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
