namespace HjemIKroppenBooking.Api.Contracts.Requests.Series;

public sealed record AdminCreateSeriesBookingRequest(Guid UserId, string? Note);
