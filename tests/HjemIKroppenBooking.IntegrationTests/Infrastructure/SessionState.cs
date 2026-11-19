namespace HjemIKroppenBooking.IntegrationTests.Infrastructure;

/// <summary>A session's capacity figures as stored in the database.</summary>
public sealed record SessionState(int Capacity, int ActiveBookingCount, int ActiveBookingRows);
