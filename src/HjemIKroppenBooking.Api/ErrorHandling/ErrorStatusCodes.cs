using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>Maps transport-neutral error types to HTTP status codes, and bare HTTP errors to error codes.</summary>
public static class ErrorStatusCodes
{
    public static int FromErrorType(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };

    /// <summary>The error code used for responses produced by the framework, e.g. 401 from authentication or 404 for an unknown route.</summary>
    public static string DefaultErrorCode(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => ErrorCodes.RequestInvalid,
        StatusCodes.Status401Unauthorized => ErrorCodes.AuthUnauthorized,
        StatusCodes.Status403Forbidden => ErrorCodes.AuthForbidden,
        StatusCodes.Status404NotFound => ErrorCodes.RequestNotFound,
        StatusCodes.Status405MethodNotAllowed => ErrorCodes.RequestMethodNotAllowed,
        StatusCodes.Status415UnsupportedMediaType => ErrorCodes.RequestUnsupportedMediaType,
        StatusCodes.Status422UnprocessableEntity => ErrorCodes.ValidationFailed,
        StatusCodes.Status429TooManyRequests => ErrorCodes.RequestRateLimited,
        _ => ErrorCodes.ServerUnexpectedError
    };
}
