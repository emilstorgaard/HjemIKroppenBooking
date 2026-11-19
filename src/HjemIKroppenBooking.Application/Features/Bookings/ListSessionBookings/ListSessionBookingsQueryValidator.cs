using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.Bookings.ListSessionBookings;

public sealed class ListSessionBookingsQueryValidator : AppValidator<ListSessionBookingsQuery>
{
    public ListSessionBookingsQueryValidator() => Include(new PagingValidator());
}
