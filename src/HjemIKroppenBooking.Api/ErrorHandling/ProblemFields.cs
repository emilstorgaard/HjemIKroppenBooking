namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>Names of the extension members added to every problem response.</summary>
public static class ProblemFields
{
    public const string ContentType = "application/problem+json";
    public const string Code = "code";
    public const string TraceId = "traceId";
    public const string CorrelationId = "correlationId";
    public const string Errors = "errors";
    public const string Parameters = "parameters";
}
