namespace HjemIKroppenBooking.Application.Contracts.Responses.Series;

public sealed record AdminSeriesSessionResponse(
    Guid Id, DateTime StartsAt, DateTime EndsAt, string? Location, int Capacity, int ActiveBookingCount, bool IsCancelled);
