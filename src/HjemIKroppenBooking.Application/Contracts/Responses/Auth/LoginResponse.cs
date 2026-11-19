using HjemIKroppenBooking.Application.Contracts.Responses.Users;

namespace HjemIKroppenBooking.Application.Contracts.Responses.Auth;

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt, UserResponse User);
