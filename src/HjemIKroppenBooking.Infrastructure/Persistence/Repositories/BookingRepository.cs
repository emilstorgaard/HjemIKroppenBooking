using HjemIKroppenBooking.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Repositories;

internal sealed class BookingRepository(AppDbContext db) : IBookingRepository
{
    public Task<BookingKey?> FindKeyAsync(Guid bookingId, CancellationToken cancellationToken) =>
        db.Bookings.AsNoTracking()
            .Where(b => b.Id == bookingId)
            .Select(b => new BookingKey(b.Id, b.SessionId, b.UserId, b.Status))
            .SingleOrDefaultAsync(cancellationToken);
}
