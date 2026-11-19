using System.Net;

namespace HjemIKroppenBooking.Api.Options;

/// <summary>
/// Reverse proxies whose <c>X-Forwarded-For</c>/<c>X-Forwarded-Proto</c> headers are trusted, bound from the
/// <c>ForwardedHeaders</c> section: IP addresses in <c>KnownProxies</c> and CIDR ranges in <c>KnownNetworks</c>.
/// Without any, only loopback is trusted and the client IP is the proxy's IP.
/// </summary>
public sealed class ForwardedHeadersSettings
{
    public const string SectionName = "ForwardedHeaders";

    public IList<string> KnownProxies { get; } = new List<string>();

    public IList<string> KnownNetworks { get; } = new List<string>();

    public bool IsConfigured => KnownProxies.Count > 0 || KnownNetworks.Count > 0;

    public bool IsValid() => KnownProxies.All(p => IPAddress.TryParse(p, out _)) && KnownNetworks.All(n => IPNetwork.TryParse(n, out _));
}
