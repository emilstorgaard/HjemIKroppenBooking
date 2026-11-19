using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Features.Bookings.CreateBookingForUser;

public sealed class CreateBookingForUserCommandValidator : AppValidator<CreateBookingForUserCommand>
{
    public CreateBookingForUserCommandValidator()
    {
        RuleFor(x => x.UserId).RequiredId();
        RuleFor(x => x.SessionId).RequiredId();
        RuleFor(x => x.Note).OptionalText(Booking.NoteMaxLength);
    }
}
