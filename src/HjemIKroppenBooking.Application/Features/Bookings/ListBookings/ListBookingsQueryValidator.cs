using FluentValidation;
using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.ListBookings;

public sealed class ListBookingsQueryValidator : AppValidator<ListBookingsQuery>
{
    public ListBookingsQueryValidator()
    {
        Include(new PagingValidator());
        RuleFor(x => x.Status).IsInEnum().WithErrorCode(ErrorCodes.ValidationEnumInvalid);
        RuleFor(x => x.To).AfterFrom(x => x.From);
    }
}
