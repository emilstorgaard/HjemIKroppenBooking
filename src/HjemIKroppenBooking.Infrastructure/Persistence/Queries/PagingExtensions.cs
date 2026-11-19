using System.Linq.Expressions;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Queries;

internal static class PagingExtensions
{
    /// <summary>
    /// Counts the filtered rows, then sorts, pages and projects in the database. <paramref name="order"/> must give a
    /// total order (end with a unique key) so pages are stable.
    /// </summary>
    public static async Task<PagedResponse<TResult>> ToPagedResponseAsync<TSource, TResult>(
        this IQueryable<TSource> filtered,
        Func<IQueryable<TSource>, IOrderedQueryable<TSource>> order,
        Expression<Func<TSource, TResult>> projection,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await filtered.CountAsync(cancellationToken);
        IReadOnlyList<TResult> items = totalCount == 0
            ? []
            : await order(filtered).Skip((page - 1) * pageSize).Take(pageSize).Select(projection).ToListAsync(cancellationToken);
        return new PagedResponse<TResult>(items, page, pageSize, totalCount);
    }
}
