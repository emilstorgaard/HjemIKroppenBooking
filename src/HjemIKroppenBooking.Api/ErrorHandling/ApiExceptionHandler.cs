using HjemIKroppenBooking.Application.Common.Validation;
using HjemIKroppenBooking.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>
/// Turns exceptions into problem responses. Expected errors carry an error code and are mapped by error type;
/// anything else is logged and returned as a generic 500 without exception details.
/// </summary>
internal sealed partial class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            // The client went away; there is nobody to answer.
            httpContext.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            return true;
        }

        var problem = CreateProblem(exception);
        if (problem is null)
        {
            LogUnexpectedError(logger, exception, httpContext.Request.Method, httpContext.Request.Path);
            problem = Problems.Error(StatusCodes.Status500InternalServerError, ErrorCodes.ServerUnexpectedError);
        }

        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        await ProblemWriter.WriteAsync(httpContext, problem, cancellationToken, exception);
        return true;
    }

    private static ProblemDetails? CreateProblem(Exception exception)
    {
        if (exception is RequestValidationException validation)
        {
            return Problems.Validation(validation.Errors.Select(e => (e.Field, e.Code)));
        }

        if (exception is DomainException domain)
        {
            // Domain validation errors that name a field are reported like validator errors, so forms can show them.
            if (domain.ErrorType == ErrorType.Validation && domain.Parameters.TryGetValue("field", out var value) && value is string field)
            {
                return Problems.Validation([(field, domain.Code)]);
            }

            return Problems.Error(ErrorStatusCodes.FromErrorType(domain.ErrorType), domain.Code, domain.Parameters);
        }

        if (exception is BadHttpRequestException badRequest)
        {
            return Problems.Error(badRequest.StatusCode, ErrorStatusCodes.DefaultErrorCode(badRequest.StatusCode));
        }

        return null;
    }

    [LoggerMessage(EventId = 5000, Level = LogLevel.Error, Message = "Unhandled exception for {Method} {Path}")]
    private static partial void LogUnexpectedError(ILogger logger, Exception exception, string method, string path);
}
