using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.ServiceTypes;
using HjemIKroppenBooking.Domain.Sessions;
using HjemIKroppenBooking.Domain.Users;
using HjemIKroppenBooking.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HjemIKroppenBooking.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, TimeProvider timeProvider) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();

    public DbSet<SessionSeries> SessionSeries => Set<SessionSeries>();

    public DbSet<Session> Sessions => Set<Session>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        StampEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Ids are assigned by the domain (Entity constructor), never by the database. Without this, EF Core treats a new
        // entity with a non-empty Guid key that is reached through a navigation (a booking added to Session.Bookings) as an
        // existing row, issues an UPDATE instead of an INSERT, and the save fails as a concurrency conflict.
        var idProperties = modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(Entity).IsAssignableFrom(entityType.ClrType))
            .Select(entityType => entityType.FindProperty(nameof(Entity.Id)))
            .OfType<IMutableProperty>()
            .ToList();
        foreach (var property in idProperties)
        {
            property.ValueGenerated = ValueGenerated.Never;
        }

        // All timestamps are UTC. Applied explicitly to DateTime and DateTime? so values read back always have Kind=Utc.
        var converter = new UtcDateTimeConverter();
        var dateTimeProperties = modelBuilder.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties())
            .Where(property => property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
            .ToList();
        foreach (var property in dateTimeProperties)
        {
            property.SetValueConverter(converter);
        }
    }

    private void StampEntities()
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(e => e.CreatedAtUtc).CurrentValue = now;
                    entry.Property(e => e.UpdatedAtUtc).CurrentValue = now;
                    break;
                case EntityState.Modified:
                    entry.Property(e => e.UpdatedAtUtc).CurrentValue = now;
                    entry.Property(e => e.CreatedAtUtc).IsModified = false;
                    break;
            }
        }
    }
}
