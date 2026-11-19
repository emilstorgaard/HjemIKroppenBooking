using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>
/// Writes a problem from MVC through <see cref="ProblemWriter"/>. An <c>ObjectResult</c> would get its content type
/// replaced by the controllers' <c>[Produces("application/json")]</c>; this keeps <c>application/problem+json</c>.
/// </summary>
public sealed class ProblemResult(ProblemDetails problem) : IActionResult
{
    public Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.HttpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status400BadRequest;
        return ProblemWriter.WriteAsync(context.HttpContext, problem, context.HttpContext.RequestAborted);
    }
}
