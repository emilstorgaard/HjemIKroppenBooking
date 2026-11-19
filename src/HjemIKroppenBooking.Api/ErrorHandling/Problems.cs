using System.Diagnostics;
using HjemIKroppenBooking.Api.Middleware;
using HjemIKroppenBooking.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>Builds the problem responses. Every problem gets a code, a Danish detail, the trace id and the correlation id.</summary>
public static class Problems
{
    public static ProblemDetails Error(int statusCode, string code, IReadOnlyDictionary<string, object?>? parameters = null)
    {
        var problem = new ProblemDetails { Status = statusCode, Detail = ErrorCatalog.GetMessage(code) };
        problem.Extensions[ProblemFields.Code] = code;

        var extra = parameters?.Where(p => !string.Equals(p.Key, "field", StringComparison.Ordinal)).ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal);
        if (extra is { Count: > 0 })
        {
            problem.Extensions[ProblemFields.Parameters] = extra;
        }

        return problem;
    }

    /// <summary>A validation problem with a list of <see cref="FieldError"/> per camelCase field path.</summary>
    public static ProblemDetails Validation(IEnumerable<(string Field, string Code)> errors, int statusCode = StatusCodes.Status422UnprocessableEntity, string code = ErrorCodes.ValidationFailed)
    {
        var problem = Error(statusCode, code);
        problem.Extensions[ProblemFields.Errors] = errors
            .GroupBy(e => e.Field, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.Select(e => new FieldError(e.Code, ErrorCatalog.GetMessage(e.Code))).Distinct().ToList(), StringComparer.Ordinal);
        return problem;
    }

    /// <summary>
    /// Completes a problem: instance, trace id (W3C) and correlation id. Problems created by the framework (401, 404,
    /// 405 ...) have no code yet; they get the default code and Danish message for their status.
    /// </summary>
    public static void Enrich(ProblemDetails problem, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(problem);
        ArgumentNullException.ThrowIfNull(httpContext);

        problem.Status ??= httpContext.Response.StatusCode;
        problem.Instance ??= httpContext.Request.Path;
        if (!problem.Extensions.ContainsKey(ProblemFields.Code))
        {
            var code = ErrorStatusCodes.DefaultErrorCode(problem.Status.Value);
            problem.Extensions[ProblemFields.Code] = code;
            problem.Detail = ErrorCatalog.GetMessage(code);
        }

        problem.Extensions[ProblemFields.TraceId] = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
        problem.Extensions[ProblemFields.CorrelationId] = CorrelationIdMiddleware.GetCorrelationId(httpContext);
    }
}
