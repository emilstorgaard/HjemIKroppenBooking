namespace HjemIKroppenBooking.Api.Controllers;

public static class ApiRoutes
{
    /// <summary>Route prefix for versioned controllers, e.g. <c>/api/v1/sessions</c>.</summary>
    public const string Prefix = "api/v{version:apiVersion}";

    /// <summary>Absolute path prefix for version 1, used to build <c>Location</c> headers.</summary>
    public const string V1 = "/api/v1";

    /// <summary>Formats a path below <see cref="V1"/> with the invariant culture.</summary>
    public static string V1Path(FormattableString path) => V1 + FormattableString.Invariant(path);
}
