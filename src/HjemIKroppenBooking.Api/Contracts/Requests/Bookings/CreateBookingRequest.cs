namespace HjemIKroppenBooking.Api.Contracts.Requests.Bookings;

public sealed record CreateBookingRequest(Guid SessionId, string? Note);
