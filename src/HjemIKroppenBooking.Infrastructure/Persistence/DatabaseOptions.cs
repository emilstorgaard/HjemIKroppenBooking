using System.ComponentModel.DataAnnotations;

namespace HjemIKroppenBooking.Infrastructure.Persistence;

/// <summary>Bound from <c>ConnectionStrings:Default</c> (environment variable <c>ConnectionStrings__Default</c>).</summary>
public sealed class DatabaseOptions
{
    public const string ConnectionStringName = "Default";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}
