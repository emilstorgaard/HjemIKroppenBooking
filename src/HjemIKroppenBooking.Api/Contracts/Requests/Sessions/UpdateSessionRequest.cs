namespace HjemIKroppenBooking.Api.Contracts.Requests.Sessions;

public sealed record UpdateSessionRequest(DateTimeOffset StartsAt, DateTimeOffset EndsAt, int Capacity, string? Location, string? Notes);
