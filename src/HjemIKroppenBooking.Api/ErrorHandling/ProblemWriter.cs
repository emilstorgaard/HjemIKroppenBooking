using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>
/// Writes a problem response through <see cref="IProblemDetailsService"/>. If the client's <c>Accept</c> header
/// rules out JSON the service declines, and the problem is written as <c>application/problem+json</c> anyway.
/// </summary>
public static class ProblemWriter
{
    public static async Task WriteAsync(HttpContext httpContext, ProblemDetails problem, CancellationToken cancellationToken, Exception? exception = null)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(problem);
        Problems.Enrich(problem, httpContext);

        var service = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
        var context = new ProblemDetailsContext { HttpContext = httpContext, ProblemDetails = problem, Exception = exception };
        if (!await service.TryWriteAsync(context))
        {
            await httpContext.Response.WriteAsJsonAsync(problem, options: null, contentType: ProblemFields.ContentType, cancellationToken);
        }
    }
}
