using HjemIKroppenBooking.Domain.Sessions;
using HjemIKroppenBooking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Configurations;

internal sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    /// <summary>Shadow property holding the SQL Server rowversion used for optimistic concurrency.</summary>
    public const string RowVersion = "RowVersion";

    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.Property(x => x.Location).HasMaxLength(Session.LocationMaxLength);
        builder.Property(x => x.Notes).HasMaxLength(Session.NotesMaxLength);

        // Every booking and cancellation changes ActiveBookingCount, so the session row is updated and the row version
        // is checked. Two concurrent bookings of the last spot therefore cannot both succeed.
        builder.Property<byte[]>(RowVersion).IsRowVersion();

        builder.Ignore(x => x.AvailableSpots);
        builder.Ignore(x => x.IsFull);

        builder.HasOne(x => x.ServiceType).WithMany().HasForeignKey(x => x.ServiceTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

        // Deleting a session (only allowed without active bookings) also deletes its cancelled bookings.
        builder.HasMany(x => x.Bookings).WithOne(x => x.Session).HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Bookings).HasField("_bookings").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.StartsAtUtc);
    }
}
