using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Configurations;

internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Note).HasMaxLength(Booking.NoteMaxLength);
        builder.Ignore(x => x.IsActive);

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);

        // Database guarantee for "at most one active booking per user per session". A cancelled booking does not block
        // a new one. The filter is the one raw SQL string in the model, because EF cannot express filters otherwise.
        builder.HasIndex(x => new { x.SessionId, x.UserId })
            .IsUnique()
            .HasFilter($"[Status] = '{nameof(BookingStatus.Active)}'")
            .HasDatabaseName(IndexNames.ActiveBookingPerUser);

        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}
