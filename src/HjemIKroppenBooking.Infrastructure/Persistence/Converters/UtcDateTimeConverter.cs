using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Converters;

/// <summary>
/// SQL Server <c>datetime2</c> has no time zone. Everything is stored as UTC, so values read back are marked as UTC
/// and local values are converted before they are written.
/// </summary>
public sealed class UtcDateTimeConverter()
    : ValueConverter<DateTime, DateTime>(
        value => value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : value,
        value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
