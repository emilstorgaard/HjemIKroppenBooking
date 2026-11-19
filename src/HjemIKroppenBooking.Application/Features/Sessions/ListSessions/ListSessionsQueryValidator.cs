using FluentValidation;
using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Features.Sessions.ListSessions;

public sealed class ListSessionsQueryValidator : AppValidator<ListSessionsQuery>
{
    public ListSessionsQueryValidator()
    {
        Include(new PagingValidator());
        RuleFor(x => x.Category).IsInEnum().WithErrorCode(ErrorCodes.ValidationEnumInvalid);
        RuleFor(x => x.To).AfterFrom(x => x.From);
    }
}
