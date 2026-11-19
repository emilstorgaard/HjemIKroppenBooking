using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Features.Bookings.CreateBooking;

public sealed class CreateBookingCommandValidator : AppValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.SessionId).RequiredId();
        RuleFor(x => x.Note).OptionalText(Booking.NoteMaxLength);
    }
}
