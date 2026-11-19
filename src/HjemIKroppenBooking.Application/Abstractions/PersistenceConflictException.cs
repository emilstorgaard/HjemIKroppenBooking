namespace HjemIKroppenBooking.Application.Abstractions;

/// <summary>
/// Thrown by <see cref="IUnitOfWork"/> when a save fails because another request changed the same data
/// (row version conflict) or because a unique index was violated. <see cref="ErrorCode"/> is the code reported to
/// the client if the retried command fails again.
/// </summary>
public sealed class PersistenceConflictException : Exception
{
    public PersistenceConflictException(string errorCode, Exception? innerException)
        : base($"Persistence conflict: {errorCode}", innerException)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }
}
