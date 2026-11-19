using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>
/// Response for requests MVC could not bind (malformed JSON, wrong types, unknown enum values, missing body):
/// 400 with code <c>request.invalid</c> and <c>validation.invalid_value</c> per field.
/// </summary>
public static class InvalidModelStateResponse
{
    public static IActionResult Create(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var errors = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .Select(entry => (Field: ToFieldPath(entry.Key), Code: ErrorCodes.ValidationInvalidValue));

        return new ProblemResult(Problems.Validation(errors, StatusCodes.Status400BadRequest, ErrorCodes.RequestInvalid));
    }

    /// <summary>JSON paths arrive as <c>$.startsAt</c>; query and route keys as <c>Page</c>.</summary>
    private static string ToFieldPath(string key) =>
        ValidationError.ToFieldPath(key.StartsWith("$.", StringComparison.Ordinal) ? key[2..] : key.TrimStart('$'));
}
