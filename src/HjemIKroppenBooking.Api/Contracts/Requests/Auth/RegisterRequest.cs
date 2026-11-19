namespace HjemIKroppenBooking.Api.Contracts.Requests.Auth;

public sealed record RegisterRequest(string Email, string FullName, string Password);
