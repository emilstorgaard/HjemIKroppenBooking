using System.Runtime.CompilerServices;

namespace HjemIKroppenBooking.Domain.Common;

/// <summary>Shared argument checks for the aggregates.</summary>
internal static class Guard
{
    /// <summary>Rejects non-UTC values. A wrong kind is a programming error, not user input.</summary>
    public static DateTime Utc(DateTime value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("The value must be a UTC DateTime.", paramName);
        }

        return value;
    }

    /// <summary>Trims the text and requires it to be non-empty and within the maximum length.</summary>
    public static string RequiredText(string? value, int maxLength, string field)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            throw DomainException.Validation(ErrorCodes.ValidationRequired, Field(field));
        }

        return WithinMaxLength(trimmed, maxLength, field);
    }

    /// <summary>Trims the text and returns null for empty input.</summary>
    public static string? OptionalText(string? value, int maxLength, string field)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrEmpty(trimmed) ? null : WithinMaxLength(trimmed, maxLength, field);
    }

    public static void InRange(int value, int min, int max, string code, string field)
    {
        if (value < min || value > max)
        {
            throw DomainException.Validation(code, Field(field, ("min", min), ("max", max)));
        }
    }

    public static Dictionary<string, object?> Field(string field, params (string Key, object? Value)[] extra)
    {
        var parameters = new Dictionary<string, object?>(StringComparer.Ordinal) { ["field"] = field };
        foreach (var (key, value) in extra)
        {
            parameters[key] = value;
        }

        return parameters;
    }

    private static string WithinMaxLength(string value, int maxLength, string field) =>
        value.Length <= maxLength
            ? value
            : throw DomainException.Validation(ErrorCodes.ValidationMaxLength, Field(field, ("maxLength", maxLength)));
}
