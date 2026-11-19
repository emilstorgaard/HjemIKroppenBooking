using FluentValidation;

namespace HjemIKroppenBooking.Application.Common.Validation;

/// <summary>
/// Base class for all validators. Stops at the first failing rule per property, so each field reports one error
/// code and later rules never see values an earlier rule rejected (e.g. null).
/// </summary>
public abstract class AppValidator<T> : AbstractValidator<T>
{
    protected AppValidator() => RuleLevelCascadeMode = CascadeMode.Stop;
}
