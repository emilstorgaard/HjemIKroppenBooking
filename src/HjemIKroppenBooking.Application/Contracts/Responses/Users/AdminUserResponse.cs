using HjemIKroppenBooking.Domain.Users;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Users;

public sealed record AdminUserResponse(Guid Id, string Email, string FullName, UserRole Role, bool IsActive, DateTime CreatedAt, DateTime UpdatedAt);
