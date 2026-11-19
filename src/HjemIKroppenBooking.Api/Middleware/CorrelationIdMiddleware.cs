using System.Diagnostics;

namespace HjemIKroppenBooking.Api.Middleware;

/// <summary>
/// Accepts an incoming <c>X-Correlation-Id</c> or falls back to the W3C trace id, echoes it in the response and adds it
/// to the logging scope, so one request can be followed across logs and error responses.
/// </summary>
public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-Id";
    private const int MaxLength = 128;
    private static readonly object ItemKey = new();
    private static readonly Func<ILogger, string, IDisposable?> CorrelationScope = LoggerMessage.DefineScope<string>("CorrelationId:{CorrelationId}");

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var correlationId = IsValid(context.Request.Headers[HeaderName].ToString())
            ? context.Request.Headers[HeaderName].ToString()
            : Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        context.Items[ItemKey] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (CorrelationScope(logger, correlationId))
        {
            await next(context);
        }
    }

    public static string GetCorrelationId(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Items.TryGetValue(ItemKey, out var value) && value is string id ? id : Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
    }

    /// <summary>Only short, printable ids are echoed, so the header cannot be used to inject content into logs or responses.</summary>
    private static bool IsValid(string value) =>
        value.Length is > 0 and <= MaxLength && value.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.' or ':');
}
