namespace HjemIKroppenBooking.Api.Options;

/// <summary>
/// Browser origins allowed to call the API, bound from the top-level <c>AllowedOrigins</c> list
/// (environment variables <c>AllowedOrigins__0</c>, <c>AllowedOrigins__1</c> ...).
/// </summary>
public sealed class CorsSettings
{
    public IList<string> AllowedOrigins { get; } = new List<string>();

    public static bool IsValidOrigin(string origin) =>
        Uri.TryCreate(origin, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) && uri.AbsolutePath == "/" && !origin.EndsWith('/');
}
