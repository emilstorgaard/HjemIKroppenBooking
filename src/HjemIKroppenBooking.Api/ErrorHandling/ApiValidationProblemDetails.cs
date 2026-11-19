namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>Documents validation problems: a list of errors per camelCase field path, e.g. <c>startTimes[0]</c>.</summary>
public sealed class ApiValidationProblemDetails : ApiProblemDetails
{
    public IReadOnlyDictionary<string, IReadOnlyList<FieldError>> Errors { get; init; } = new Dictionary<string, IReadOnlyList<FieldError>>();
}
