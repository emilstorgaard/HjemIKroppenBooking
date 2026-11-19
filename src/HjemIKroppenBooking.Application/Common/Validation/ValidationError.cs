namespace HjemIKroppenBooking.Application.Common.Validation;

/// <summary>A single invalid input: the camelCase field path (e.g. <c>startTimes[0]</c>) and an error code.</summary>
public sealed record ValidationError(string Field, string Code)
{
    /// <summary>Converts a FluentValidation property path such as <c>StartTimes[0]</c> to <c>startTimes[0]</c>.</summary>
    public static string ToFieldPath(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return string.Empty;
        }

        var segments = propertyName.Split('.');
        for (var i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];
            if (segment.Length > 0)
            {
                segments[i] = char.ToLowerInvariant(segment[0]) + segment[1..];
            }
        }

        return string.Join('.', segments);
    }
}
