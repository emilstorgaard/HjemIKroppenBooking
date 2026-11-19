namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>
/// Documents the problem response shape in OpenAPI. At runtime the responses are written as standard
/// <c>ProblemDetails</c> with the same JSON members.
/// </summary>
public class ApiProblemDetails
{
    public string? Type { get; init; }

    public string? Title { get; init; }

    public int Status { get; init; }

    /// <summary>Danish message that can be shown to the user.</summary>
    public string Detail { get; init; } = string.Empty;

    public string? Instance { get; init; }

    /// <summary>Stable machine-readable error code, e.g. <c>session.full</c>.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>W3C trace id of the request.</summary>
    public string TraceId { get; init; } = string.Empty;

    /// <summary>The <c>X-Correlation-Id</c> of the request.</summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>Optional details, e.g. <c>sessionId</c> of the full session in a series booking.</summary>
    public IReadOnlyDictionary<string, object?>? Parameters { get; init; }
}
