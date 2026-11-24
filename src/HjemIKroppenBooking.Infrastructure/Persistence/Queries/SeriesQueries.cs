using System.Linq.Expressions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Features.Series.ListBookableSeries;
using HjemIKroppenBooking.Application.Features.Series.ListSeries;
using HjemIKroppenBooking.Application.Features.Series.ListSeriesParticipants;
using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Series;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Queries;

internal sealed class SeriesQueries(AppDbContext db) : ISeriesQueries
{
    private static readonly Expression<Func<SessionSeries, AdminSeriesResponse>> ToAdmin = s => new AdminSeriesResponse(
        s.Id,
        s.Title,
        s.ServiceTypeId,
        s.ServiceType.Name,
        new ServiceCategorySummary(s.ServiceType.Category.Id, s.ServiceType.Category.Name, s.ServiceType.Category.Slug),
        s.IsCancelled,
        s.CancelledAtUtc,
        s.Sessions.SelectMany(x => x.Bookings).Where(b => b.Status == BookingStatus.Active).Select(b => b.UserId).Distinct().Count(),
        s.Sessions.Min(x => (DateTime?)x.StartsAtUtc),
        s.Sessions.Max(x => (DateTime?)x.EndsAtUtc),
        s.Sessions
            .OrderBy(x => x.StartsAtUtc)
            .ThenBy(x => x.Id)
            .Select(x => new AdminSeriesSessionResponse(x.Id, x.StartsAtUtc, x.EndsAtUtc, x.Location, x.Capacity, x.ActiveBookingCount, x.IsCancelled))
            .ToList(),
        s.CreatedAtUtc,
        s.UpdatedAtUtc);

    private static readonly Expression<Func<SessionSeries, AdminSeriesListItemResponse>> ToAdminListItem = s => new AdminSeriesListItemResponse(
        s.Id,
        s.Title,
        s.ServiceTypeId,
        s.ServiceType.Name,
        new ServiceCategorySummary(s.ServiceType.Category.Id, s.ServiceType.Category.Name, s.ServiceType.Category.Slug),
        s.IsCancelled,
        s.Sessions.Select(x => x.Id).Count(),
        s.Sessions.SelectMany(x => x.Bookings).Where(b => b.Status == BookingStatus.Active).Select(b => b.UserId).Distinct().Count(),
        s.Sessions.Min(x => (DateTime?)x.StartsAtUtc),
        s.Sessions.Max(x => (DateTime?)x.EndsAtUtc),
        s.CreatedAtUtc,
        s.UpdatedAtUtc);

    public Task<PagedResponse<SeriesResponse>> ListBookableAsync(ListBookableSeriesQuery query, DateTime nowUtc, CancellationToken cancellationToken)
    {
        var series = VisibleSeries().Where(s => s.Sessions.Any(x => !x.IsCancelled && x.StartsAtUtc > nowUtc));
        if (query.ServiceTypeId is { } serviceTypeId)
        {
            series = series.Where(s => s.ServiceTypeId == serviceTypeId);
        }

        if (query.OnlyAvailable)
        {
            series = series.Where(s => !s.Sessions.Any(x => !x.IsCancelled && x.StartsAtUtc > nowUtc && x.ActiveBookingCount >= x.Capacity));
        }

        return series.ToPagedResponseAsync(
            q => q.OrderBy(s => s.Sessions.Where(x => !x.IsCancelled && x.StartsAtUtc > nowUtc).Min(x => (DateTime?)x.StartsAtUtc)).ThenBy(s => s.Id),
            ToPublic(nowUtc),
            query.Page,
            query.PageSize,
            cancellationToken);
    }

    public Task<SeriesResponse?> GetPublicByIdAsync(Guid id, DateTime nowUtc, CancellationToken cancellationToken) =>
        VisibleSeries().Where(s => s.Id == id).Select(ToPublic(nowUtc)).SingleOrDefaultAsync(cancellationToken);

    public Task<PagedResponse<AdminSeriesListItemResponse>> ListAsync(ListSeriesQuery query, DateTime nowUtc, CancellationToken cancellationToken)
    {
        var series = db.SessionSeries.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            series = series.Where(s => s.Title.Contains(term) || s.ServiceType.Name.Contains(term));
        }

        if (query.ServiceTypeId is { } serviceTypeId)
        {
            series = series.Where(s => s.ServiceTypeId == serviceTypeId);
        }

        if (query.IsCancelled is { } isCancelled)
        {
            series = series.Where(s => s.IsCancelled == isCancelled);
        }

        if (query.Upcoming is { } upcoming)
        {
            series = upcoming
                ? series.Where(s => s.Sessions.Any(x => !x.IsCancelled && x.EndsAtUtc > nowUtc))
                : series.Where(s => !s.Sessions.Any(x => !x.IsCancelled && x.EndsAtUtc > nowUtc));
        }

        Func<IQueryable<SessionSeries>, IOrderedQueryable<SessionSeries>> order = query.Upcoming == true
            ? q => q.OrderBy(s => s.Sessions.Min(x => (DateTime?)x.StartsAtUtc)).ThenBy(s => s.Id)
            : q => q.OrderByDescending(s => s.Sessions.Min(x => (DateTime?)x.StartsAtUtc)).ThenBy(s => s.Id);
        return series.ToPagedResponseAsync(order, ToAdminListItem, query.Page, query.PageSize, cancellationToken);
    }

    public Task<AdminSeriesResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.SessionSeries.AsNoTracking().Where(s => s.Id == id).Select(ToAdmin).SingleOrDefaultAsync(cancellationToken);

    public async Task<PagedResponse<SeriesParticipantResponse>> ListParticipantsAsync(ListSeriesParticipantsQuery query, CancellationToken cancellationToken)
    {
        var participants = db.Bookings.AsNoTracking()
            .Where(b => b.Session.SeriesId == query.SeriesId && (query.IncludeCancelled || b.Status == BookingStatus.Active))
            .GroupBy(b => new { b.UserId, b.User.FullName, b.User.Email })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.FullName,
                g.Key.Email,
                ActiveBookingCount = g.Count(b => b.Status == BookingStatus.Active),
                Note = g.Max(b => b.Note),
                FirstBookedAt = g.Min(b => b.CreatedAtUtc)
            });

        var totalCount = await participants.CountAsync(cancellationToken);
        var page = await participants
            .OrderBy(p => p.FirstBookedAt)
            .ThenBy(p => p.UserId)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = page.Select(p => new SeriesParticipantResponse(p.UserId, p.FullName, p.Email, p.ActiveBookingCount, p.Note, p.FirstBookedAt)).ToList();
        return new PagedResponse<SeriesParticipantResponse>(items, query.Page, query.PageSize, totalCount);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) => db.SessionSeries.AnyAsync(s => s.Id == id, cancellationToken);

    private IQueryable<SessionSeries> VisibleSeries() => db.SessionSeries.AsNoTracking().Where(s => !s.IsCancelled && s.ServiceType.IsActive);

    /// <summary>Only non-cancelled sessions are shown. Free spots are the minimum over the sessions that have not started.</summary>
    private static Expression<Func<SessionSeries, SeriesResponse>> ToPublic(DateTime nowUtc) => s => new SeriesResponse(
        s.Id,
        s.Title,
        s.ServiceTypeId,
        s.ServiceType.Name,
        new ServiceCategorySummary(s.ServiceType.Category.Id, s.ServiceType.Category.Name, s.ServiceType.Category.Slug),
        s.Sessions.Any(x => !x.IsCancelled && x.StartsAtUtc > nowUtc),
        s.Sessions.Where(x => !x.IsCancelled && x.StartsAtUtc > nowUtc).Min(x => (int?)(x.Capacity - x.ActiveBookingCount)) ?? 0,
        s.Sessions.Where(x => !x.IsCancelled).Min(x => (DateTime?)x.StartsAtUtc),
        s.Sessions.Where(x => !x.IsCancelled).Max(x => (DateTime?)x.EndsAtUtc),
        s.Sessions
            .Where(x => !x.IsCancelled)
            .OrderBy(x => x.StartsAtUtc)
            .ThenBy(x => x.Id)
            .Select(x => new SeriesSessionResponse(x.Id, x.StartsAtUtc, x.EndsAtUtc, x.Location, x.Notes, x.Capacity, x.Capacity - x.ActiveBookingCount, x.StartsAtUtc <= nowUtc))
            .ToList());
}
