using System.Linq.Expressions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Features.Bookings.Common;
using HjemIKroppenBooking.Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Queries;

internal sealed class BookingQueries(AppDbContext db) : IBookingQueries
{
    private static readonly Expression<Func<Booking, BookingResponse>> ToOwn = b => new BookingResponse(
        b.Id,
        b.SessionId,
        b.Session.SeriesId,
        b.Session.ServiceType.Name,
        b.Session.ServiceType.Category,
        b.Session.StartsAtUtc,
        b.Session.EndsAtUtc,
        b.Session.Location,
        b.Status,
        b.Note,
        b.CreatedAtUtc,
        b.CancelledAtUtc);

    private static readonly Expression<Func<Booking, AdminBookingResponse>> ToAdmin = b => new AdminBookingResponse(
        b.Id,
        b.SessionId,
        b.Session.SeriesId,
        b.UserId,
        b.User.FullName,
        b.User.Email,
        b.Session.ServiceType.Name,
        b.Session.StartsAtUtc,
        b.Session.EndsAtUtc,
        b.Status,
        b.Note,
        b.CreatedAtUtc,
        b.CancelledAtUtc);

    public Task<PagedResponse<AdminBookingResponse>> ListAsync(BookingListFilter filter, int page, int pageSize, CancellationToken cancellationToken) =>
        Apply(filter).ToPagedResponseAsync(Order(filter), ToAdmin, page, pageSize, cancellationToken);

    public Task<PagedResponse<BookingResponse>> ListOwnAsync(BookingListFilter filter, int page, int pageSize, CancellationToken cancellationToken) =>
        Apply(filter).ToPagedResponseAsync(Order(filter), ToOwn, page, pageSize, cancellationToken);

    public Task<AdminBookingResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Bookings.AsNoTracking().Where(b => b.Id == id).Select(ToAdmin).SingleOrDefaultAsync(cancellationToken);

    public Task<BookingResponse?> GetOwnByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken) =>
        db.Bookings.AsNoTracking().Where(b => b.Id == id && b.UserId == userId).Select(ToOwn).SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<BookingResponse>> GetOwnByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken) =>
        await ByIds(ids).Select(ToOwn).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AdminBookingResponse>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken) =>
        await ByIds(ids).Select(ToAdmin).ToListAsync(cancellationToken);

    private IOrderedQueryable<Booking> ByIds(IReadOnlyCollection<Guid> ids) =>
        db.Bookings.AsNoTracking().Where(b => ids.Contains(b.Id)).OrderBy(b => b.Session.StartsAtUtc).ThenBy(b => b.Id);

    private static Func<IQueryable<Booking>, IOrderedQueryable<Booking>> Order(BookingListFilter filter) =>
        filter.Upcoming == true
            ? q => q.OrderBy(b => b.Session.StartsAtUtc).ThenBy(b => b.CreatedAtUtc).ThenBy(b => b.Id)
            : q => q.OrderByDescending(b => b.Session.StartsAtUtc).ThenBy(b => b.CreatedAtUtc).ThenBy(b => b.Id);

    private IQueryable<Booking> Apply(BookingListFilter filter)
    {
        var now = filter.NowUtc;
        var bookings = db.Bookings.AsNoTracking();

        if (filter.UserId is { } userId)
        {
            bookings = bookings.Where(b => b.UserId == userId);
        }

        if (filter.SessionId is { } sessionId)
        {
            bookings = bookings.Where(b => b.SessionId == sessionId);
        }

        if (filter.SeriesId is { } seriesId)
        {
            bookings = bookings.Where(b => b.Session.SeriesId == seriesId);
        }

        if (filter.ServiceTypeId is { } serviceTypeId)
        {
            bookings = bookings.Where(b => b.Session.ServiceTypeId == serviceTypeId);
        }

        if (filter.Status is { } status)
        {
            bookings = bookings.Where(b => b.Status == status);
        }

        if (filter.Upcoming is { } upcoming)
        {
            bookings = upcoming ? bookings.Where(b => b.Session.EndsAtUtc > now) : bookings.Where(b => b.Session.EndsAtUtc <= now);
        }

        if (filter.FromUtc is { } from)
        {
            bookings = bookings.Where(b => b.Session.StartsAtUtc >= from);
        }

        if (filter.ToUtc is { } to)
        {
            bookings = bookings.Where(b => b.Session.StartsAtUtc < to);
        }

        return bookings;
    }
}
