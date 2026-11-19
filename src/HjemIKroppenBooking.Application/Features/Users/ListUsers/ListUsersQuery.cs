using HjemIKroppenBooking.Application.Common.Paging;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Messaging;
using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Features.Users.ListUsers;

/// <summary>Sorted by full name, then id.</summary>
public sealed record ListUsersQuery(
    string? Search = null, UserRole? Role = null, bool? IsActive = null, int Page = PagingDefaults.Page, int PageSize = PagingDefaults.PageSize)
    : IQuery<PagedResponse<AdminUserResponse>>, IPagedQuery;
