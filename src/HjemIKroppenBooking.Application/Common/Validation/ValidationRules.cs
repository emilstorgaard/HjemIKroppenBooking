using System.Text;
using FluentValidation;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Common.Validation;

/// <summary>Reusable rules. Validators only ever set error codes; Danish messages are added by the API.</summary>
public static class ValidationRules
{
    public const int PasswordMinLength = 8;

    /// <summary>BCrypt only uses the first 72 bytes of a password, so longer passwords are rejected.</summary>
    public const int PasswordMaxBytes = 72;

    public const int SearchMaxLength = 100;

    public static IRuleBuilderOptions<T, string> RequiredText<T>(this IRuleBuilder<T, string> rule, int maxLength) =>
        rule.NotEmpty().WithErrorCode(ErrorCodes.ValidationRequired)
            .MaximumLength(maxLength).WithErrorCode(ErrorCodes.ValidationMaxLength);

    public static IRuleBuilderOptions<T, string?> OptionalText<T>(this IRuleBuilder<T, string?> rule, int maxLength) =>
        rule.Must(value => value is null || value.Length <= maxLength).WithErrorCode(ErrorCodes.ValidationMaxLength);

    public static IRuleBuilderOptions<T, Guid> RequiredId<T>(this IRuleBuilder<T, Guid> rule) =>
        rule.NotEmpty().WithErrorCode(ErrorCodes.ValidationRequired);

    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithErrorCode(ErrorCodes.ValidationRequired)
            .MaximumLength(User.EmailMaxLength).WithErrorCode(ErrorCodes.ValidationMaxLength)
            .Must(EmailRules.IsValid).WithErrorCode(ErrorCodes.ValidationEmailInvalid);

    public static IRuleBuilderOptions<T, string> ValidNewPassword<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithErrorCode(ErrorCodes.ValidationRequired)
            .MinimumLength(PasswordMinLength).WithErrorCode(ErrorCodes.ValidationPasswordTooShort)
            .Must(password => Encoding.UTF8.GetByteCount(password) <= PasswordMaxBytes).WithErrorCode(ErrorCodes.ValidationPasswordTooLong);

    /// <summary>Requires a <c>to</c> date to be after the <c>from</c> date when both are given.</summary>
    public static IRuleBuilderOptions<T, DateTime?> AfterFrom<T>(this IRuleBuilder<T, DateTime?> rule, Func<T, DateTime?> from) =>
        rule.Must((request, to) => to is null || from(request) is not { } start || to > start).WithErrorCode(ErrorCodes.ValidationDateRangeInvalid);
}
