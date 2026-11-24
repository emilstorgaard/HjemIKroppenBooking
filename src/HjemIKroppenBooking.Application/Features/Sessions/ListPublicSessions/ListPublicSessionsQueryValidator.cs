using FluentValidation;
using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Common.Validation;

namespace HjemIKroppenBooking.Application.Features.Sessions.ListPublicSessions;

public sealed class ListPublicSessionsQueryValidator : AppValidator<ListPublicSessionsQuery>
{
    public ListPublicSessionsQueryValidator()
    {
        Include(new PagingValidator());
        RuleFor(x => x.To).AfterFrom(x => x.From);
    }
}
