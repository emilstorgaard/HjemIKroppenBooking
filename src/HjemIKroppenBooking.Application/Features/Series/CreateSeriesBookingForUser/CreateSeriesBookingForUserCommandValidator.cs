using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Features.Series.CreateSeriesBookingForUser;

public sealed class CreateSeriesBookingForUserCommandValidator : AppValidator<CreateSeriesBookingForUserCommand>
{
    public CreateSeriesBookingForUserCommandValidator()
    {
        RuleFor(x => x.UserId).RequiredId();
        RuleFor(x => x.Note).OptionalText(Booking.NoteMaxLength);
    }
}
