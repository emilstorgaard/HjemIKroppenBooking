using System.Linq.Expressions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Features.Sessions.Common;
using HjemIKroppenBooking.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Queries;

/// <summary>Uses the denormalized ActiveBookingCount, so no bookings are loaded for lists.</summary>
internal sealed class SessionQueries(AppDbContext db) : ISessionQueries
{
    private static readonly Expression<Func<Session, SessionResponse>> ToPublic = s => new SessionResponse(
        s.Id,
        s.ServiceTypeId,
        s.ServiceType.Name,
        s.ServiceType.Category,
        s.SeriesId,
        s.Series != null ? s.Series.Title : null,
        s.StartsAtUtc,
        s.EndsAtUtc,
        s.Location,
        s.Notes,
        s.Capacity,
        s.Capacity - s.ActiveBookingCount);

    private static readonly Expression<Func<Session, AdminSessionResponse>> ToAdmin = s => new AdminSessionResponse(
        s.Id,
        s.ServiceTypeId,
        s.ServiceType.Name,
        s.ServiceType.Category,
        s.SeriesId,
        s.Series != null ? s.Series.Title : null,
        s.StartsAtUtc,
        s.EndsAtUtc,
        s.Location,
        s.Notes,
        s.Capacity,
        s.ActiveBookingCount,
        s.Capacity - s.ActiveBookingCount,
        s.IsCancelled,
        s.CancelledAtUtc,
        s.CreatedByUserId,
        s.CreatedAtUtc,
        s.UpdatedAtUtc);

    public Task<PagedResponse<SessionResponse>> ListPublicAsync(SessionListFilter filter, int page, int pageSize, CancellationToken cancellationToken)
    {
        var now = filter.NowUtc;
        var sessions = Apply(PublicSessions().Where(s => s.StartsAtUtc > now), filter);
        return sessions.ToPagedResponseAsync(q => q.OrderBy(s => s.StartsAtUtc).ThenBy(s => s.Id), ToPublic, page, pageSize, cancellationToken);
    }

    public Task<SessionResponse?> GetPublicByIdAsync(Guid id, CancellationToken cancellationToken) =>
        PublicSessions().Where(s => s.Id == id).Select(ToPublic).SingleOrDefaultAsync(cancellationToken);

    public Task<PagedResponse<AdminSessionResponse>> ListAsync(SessionListFilter filter, int page, int pageSize, CancellationToken cancellationToken)
    {
        var sessions = Apply(db.Sessions.AsNoTracking(), filter);
        Func<IQueryable<Session>, IOrderedQueryable<Session>> order = filter.Upcoming == true
            ? q => q.OrderBy(s => s.StartsAtUtc).ThenBy(s => s.Id)
            : q => q.OrderByDescending(s => s.StartsAtUtc).ThenBy(s => s.Id);
        return sessions.ToPagedResponseAsync(order, ToAdmin, page, pageSize, cancellationToken);
    }

    public Task<AdminSessionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Sessions.AsNoTracking().Where(s => s.Id == id).Select(ToAdmin).SingleOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) => db.Sessions.AnyAsync(s => s.Id == id, cancellationToken);

    private IQueryable<Session> PublicSessions() => db.Sessions.AsNoTracking().Where(s => !s.IsCancelled && s.ServiceType.IsActive);

    private static IQueryable<Session> Apply(IQueryable<Session> sessions, SessionListFilter filter)
    {
        var now = filter.NowUtc;
        if (filter.ServiceTypeId is { } serviceTypeId)
        {
            sessions = sessions.Where(s => s.ServiceTypeId == serviceTypeId);
        }

        if (filter.Category is { } category)
        {
            sessions = sessions.Where(s => s.ServiceType.Category == category);
        }

        if (filter.SeriesId is { } seriesId)
        {
            sessions = sessions.Where(s => s.SeriesId == seriesId);
        }

        if (filter.IsCancelled is { } isCancelled)
        {
            sessions = sessions.Where(s => s.IsCancelled == isCancelled);
        }

        if (filter.Upcoming is { } upcoming)
        {
            sessions = upcoming ? sessions.Where(s => s.EndsAtUtc > now) : sessions.Where(s => s.EndsAtUtc <= now);
        }

        if (filter.FromUtc is { } from)
        {
            sessions = sessions.Where(s => s.StartsAtUtc >= from);
        }

        if (filter.ToUtc is { } to)
        {
            sessions = sessions.Where(s => s.StartsAtUtc < to);
        }

        if (filter.OnlyAvailable)
        {
            sessions = sessions.Where(s => !s.IsCancelled && s.ActiveBookingCount < s.Capacity);
        }

        return sessions;
    }
}
