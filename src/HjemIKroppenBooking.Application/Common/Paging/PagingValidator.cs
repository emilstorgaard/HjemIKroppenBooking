using FluentValidation;
using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Application.Common.Paging;

/// <summary>Shared paging rules, included by every validator of a paged query.</summary>
public sealed class PagingValidator : AppValidator<IPagedQuery>
{
    public PagingValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithErrorCode(ErrorCodes.ValidationPageOutOfRange);
        RuleFor(x => x.PageSize).InclusiveBetween(1, PagingDefaults.MaxPageSize).WithErrorCode(ErrorCodes.ValidationPageSizeOutOfRange);
    }
}
