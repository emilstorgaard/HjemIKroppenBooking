namespace HjemIKroppenBooking.Api.Options;

/// <summary>The OpenAPI document and Scalar UI are always on in Development; elsewhere only when <c>OpenApi:Enabled</c> is true.</summary>
public sealed class OpenApiSettings
{
    public const string SectionName = "OpenApi";

    public bool Enabled { get; set; }
}
