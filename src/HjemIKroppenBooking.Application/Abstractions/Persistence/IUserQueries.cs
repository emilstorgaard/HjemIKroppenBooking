using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Application.Features.Users.ListUsers;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Read side for users. Projects directly into response models without tracking.</summary>
public interface IUserQueries
{
    Task<UserResponse?> GetProfileAsync(Guid userId, CancellationToken cancellationToken);

    Task<AdminUserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResponse<AdminUserResponse>> ListAsync(ListUsersQuery query, CancellationToken cancellationToken);
}
