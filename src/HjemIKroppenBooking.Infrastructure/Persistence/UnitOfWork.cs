using HjemIKroppenBooking.Application.Abstractions;
using HjemIKroppenBooking.Domain.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence;

/// <summary>
/// Saves through EF Core, which wraps one SaveChanges in one transaction. Translates row version conflicts and unique
/// index violations into <see cref="PersistenceConflictException"/> so the dispatcher can retry the command.
/// </summary>
internal sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    private const int UniqueIndexViolation = 2601;
    private const int UniqueConstraintViolation = 2627;

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new PersistenceConflictException(ErrorCodes.SessionConcurrencyConflict, exception);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: UniqueIndexViolation or UniqueConstraintViolation } sql)
        {
            throw new PersistenceConflictException(UniqueIndexErrorCode(sql.Message), exception);
        }
    }

    public void DiscardChanges() => db.ChangeTracker.Clear();

    /// <summary>Maps the violated index (named in the SQL error message) to the error code the client sees.</summary>
    private static string UniqueIndexErrorCode(string message)
    {
        if (message.Contains(IndexNames.ActiveBookingPerUser, StringComparison.Ordinal))
        {
            return ErrorCodes.BookingAlreadyExists;
        }

        if (message.Contains(IndexNames.UserEmail, StringComparison.Ordinal))
        {
            return ErrorCodes.UserEmailAlreadyExists;
        }

        if (message.Contains(IndexNames.ServiceTypeName, StringComparison.Ordinal))
        {
            return ErrorCodes.ServiceTypeNameAlreadyExists;
        }

        return ErrorCodes.ServerUnexpectedError;
    }
}
