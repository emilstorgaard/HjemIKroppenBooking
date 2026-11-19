namespace HjemIKroppenBooking.Api.Middleware;

/// <summary>
/// Adds security headers to every response, including error responses (the headers are set when the response starts,
/// after the exception handler has cleared the response). The API only returns JSON, so the content security policy
/// blocks everything. The interactive API documentation (<c>/scalar</c>) needs scripts and is excluded from the policy.
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var isDocumentation = context.Request.Path.StartsWithSegments("/scalar", StringComparison.OrdinalIgnoreCase);
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            headers.XContentTypeOptions = "nosniff";
            headers.XFrameOptions = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            if (!isDocumentation)
            {
                headers.ContentSecurityPolicy = "default-src 'none'; frame-ancestors 'none'";
            }

            return Task.CompletedTask;
        });

        return next(context);
    }
}
