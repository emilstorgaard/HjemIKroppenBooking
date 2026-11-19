namespace HjemIKroppenBooking.Domain.Common;

/// <summary>
/// Base class for all persisted entities. The id is generated in code so it is known before the row is saved.
/// The UTC timestamps are maintained by the persistence layer on save.
/// </summary>
public abstract class Entity
{
    protected Entity() => Id = Guid.NewGuid();

    public Guid Id { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }
}
