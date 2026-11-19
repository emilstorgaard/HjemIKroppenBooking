using System.Text.Json;
using HjemIKroppenBooking.Api.ErrorHandling;

namespace HjemIKroppenBooking.IntegrationTests.Infrastructure;

/// <summary>The problem response as a client sees it.</summary>
public sealed record ProblemResponse(
    int Status,
    string Detail,
    string Code,
    string? TraceId,
    string? CorrelationId,
    IReadOnlyDictionary<string, JsonElement>? Parameters,
    IReadOnlyDictionary<string, IReadOnlyList<FieldError>>? Errors);
