namespace HjemIKroppenBooking.Domain.Common;

/// <summary>Transport-neutral classification of an error. The API layer maps it to an HTTP status code.</summary>
public enum ErrorType
{
    Validation = 0,
    NotFound = 1,
    Conflict = 2,
    Forbidden = 3,
    Unauthorized = 4
}
