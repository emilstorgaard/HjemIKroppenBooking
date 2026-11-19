using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Application.Features.Sessions.UpdateSession;

public sealed class UpdateSessionCommandValidator : AppValidator<UpdateSessionCommand>
{
    public UpdateSessionCommandValidator()
    {
        RuleFor(x => x.EndsAt)
            .GreaterThan(x => x.StartsAt).WithErrorCode(ErrorCodes.ValidationEndBeforeStart)
            .Must((command, end) => IsValidDuration(end - command.StartsAt)).WithErrorCode(ErrorCodes.ValidationDurationOutOfRange);
        RuleFor(x => x.Capacity).InclusiveBetween(Session.MinCapacity, Session.MaxCapacity).WithErrorCode(ErrorCodes.ValidationCapacityOutOfRange);
        RuleFor(x => x.Location).OptionalText(Session.LocationMaxLength);
        RuleFor(x => x.Notes).OptionalText(Session.NotesMaxLength);
    }

    private static bool IsValidDuration(TimeSpan duration) =>
        duration >= TimeSpan.FromMinutes(Session.MinDurationMinutes) && duration <= TimeSpan.FromMinutes(Session.MaxDurationMinutes);
}
