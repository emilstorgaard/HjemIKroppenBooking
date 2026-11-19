namespace HjemIKroppenBooking.Api.ErrorHandling;

/// <summary>One validation error for a field: the machine-readable code and the Danish message.</summary>
public sealed record FieldError(string Code, string Message);
