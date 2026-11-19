using FluentValidation;
using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Bookings.ListMyBookings;

public sealed class ListMyBookingsQueryValidator : AppValidator<ListMyBookingsQuery>
{
    public ListMyBookingsQueryValidator()
    {
        Include(new PagingValidator());
        RuleFor(x => x.Status).IsInEnum().WithErrorCode(ErrorCodes.ValidationEnumInvalid);
    }
}
