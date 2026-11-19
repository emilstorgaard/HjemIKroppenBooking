using System.Linq.Expressions;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Features.Users.ListUsers;
using HjemIKroppenBooking.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Queries;

internal sealed class UserQueries(AppDbContext db) : IUserQueries
{
    private static readonly Expression<Func<User, AdminUserResponse>> ToAdmin =
        u => new AdminUserResponse(u.Id, u.Email, u.FullName, u.Role, u.IsActive, u.CreatedAtUtc, u.UpdatedAtUtc);

    public Task<UserResponse?> GetProfileAsync(Guid userId, CancellationToken cancellationToken) =>
        db.Users.AsNoTracking().Where(u => u.Id == userId).Select(u => new UserResponse(u.Id, u.Email, u.FullName, u.Role)).SingleOrDefaultAsync(cancellationToken);

    public Task<AdminUserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Users.AsNoTracking().Where(u => u.Id == id).Select(ToAdmin).SingleOrDefaultAsync(cancellationToken);

    public Task<PagedResponse<AdminUserResponse>> ListAsync(ListUsersQuery query, CancellationToken cancellationToken)
    {
        var users = db.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            users = users.Where(u => u.FullName.Contains(term) || u.Email.Contains(term));
        }

        if (query.Role is { } role)
        {
            users = users.Where(u => u.Role == role);
        }

        if (query.IsActive is { } isActive)
        {
            users = users.Where(u => u.IsActive == isActive);
        }

        return users.ToPagedResponseAsync(q => q.OrderBy(u => u.FullName).ThenBy(u => u.Id), ToAdmin, query.Page, query.PageSize, cancellationToken);
    }
}
