using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Bookings;

namespace HjemIKroppenBooking.Application.Features.Series.CreateSeriesBooking;

public sealed class CreateSeriesBookingCommandValidator : AppValidator<CreateSeriesBookingCommand>
{
    public CreateSeriesBookingCommandValidator()
    {
        RuleFor(x => x.Note).OptionalText(Booking.NoteMaxLength);
    }
}
