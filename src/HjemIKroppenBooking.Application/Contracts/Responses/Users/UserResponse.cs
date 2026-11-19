using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Users;

/// <summary>The signed-in user's own profile.</summary>
public sealed record UserResponse(Guid Id, string Email, string FullName, UserRole Role);
