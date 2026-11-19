using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;

namespace HjemIKroppenBooking.Application.Features.Users.ListUsers;

public sealed class ListUsersQueryHandler(IUserQueries users) : IQueryHandler<ListUsersQuery, PagedResponse<AdminUserResponse>>
{
    public Task<PagedResponse<AdminUserResponse>> Handle(ListUsersQuery query, CancellationToken cancellationToken) => users.ListAsync(query, cancellationToken);
}
