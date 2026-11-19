namespace HjemIKroppenBooking.Application.Abstractions;

public interface IUnitOfWork
{
    /// <summary>
    /// Saves all tracked changes in one database transaction. Concurrency conflicts and unique index violations are
    /// thrown as <see cref="PersistenceConflictException"/>, which the dispatcher retries once.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>Forgets all tracked entities so a retried command reloads its aggregates from the database.</summary>
    void DiscardChanges();
}
