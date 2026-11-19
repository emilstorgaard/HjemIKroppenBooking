using System.Collections.ObjectModel;

namespace HjemIKroppenBooking.Domain.Common;

/// <summary>
/// An expected business error. Carries a stable error code from <see cref="ErrorCodes"/>, an
/// <see cref="Common.ErrorType"/> and optional parameters. It never carries HTTP status codes or user-facing
/// text; the API layer translates the code into a Danish message.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string code, ErrorType errorType, IReadOnlyDictionary<string, object?>? parameters = null)
        : base(code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code;
        ErrorType = errorType;
        Parameters = parameters ?? ReadOnlyDictionary<string, object?>.Empty;
    }

    public string Code { get; }

    public ErrorType ErrorType { get; }

    /// <summary>
    /// Optional details, e.g. the id of a full session. A <c>field</c> entry names the input the error refers to.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Parameters { get; }

    public static DomainException NotFound(string code) => new(code, ErrorType.NotFound);

    public static DomainException Conflict(string code, IReadOnlyDictionary<string, object?>? parameters = null) =>
        new(code, ErrorType.Conflict, parameters);

    public static DomainException Validation(string code, IReadOnlyDictionary<string, object?>? parameters = null) =>
        new(code, ErrorType.Validation, parameters);

    public static DomainException Forbidden(string code) => new(code, ErrorType.Forbidden);

    public static DomainException Unauthorized(string code) => new(code, ErrorType.Unauthorized);
}
