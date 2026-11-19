namespace HjemIKroppenBooking.Api.Contracts.Requests.Bookings;

public sealed record AdminCreateBookingRequest(Guid UserId, Guid SessionId, string? Note);
