namespace HjemIKroppenBooking.Application.Common.Validation;

/// <summary>Thrown by the dispatcher when one or more validators reject a request.</summary>
public sealed class RequestValidationException : Exception
{
    public RequestValidationException(IReadOnlyList<ValidationError> errors)
        : base("Request validation failed.")
    {
        ArgumentNullException.ThrowIfNull(errors);
        Errors = errors;
    }

    public IReadOnlyList<ValidationError> Errors { get; }
}
