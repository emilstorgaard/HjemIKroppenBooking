namespace HjemIKroppenBooking.Application.Abstractions;

/// <summary>A signed access token and its expiry time in UTC.</summary>
public sealed record AccessToken(string Token, DateTime ExpiresAtUtc);
