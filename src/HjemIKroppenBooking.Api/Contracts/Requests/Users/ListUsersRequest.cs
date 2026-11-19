using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Api.Contracts.Requests.Users;

public sealed class ListUsersRequest : PagedRequest
{
    /// <summary>Matches name or email.</summary>
    public string? Search { get; init; }

    public UserRole? Role { get; init; }

    public bool? IsActive { get; init; }
}
