using System.Net;
using System.Net.Http.Json;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.IntegrationTests.Infrastructure;

namespace HjemIKroppenBooking.IntegrationTests.Sessions;

public sealed class AdminSessionTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateSession_OverlappingAnotherSession_Succeeds()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var serviceTypeId = await CreateGroupServiceTypeAsync(adminClient);
        var start = Now.AddDays(1);
        await CreateSessionAsync(adminClient, serviceTypeId, start, capacity: 10);

        // Act
        using var sameTime = await PostSessionAsync(adminClient, serviceTypeId, start, capacity: 10);
        using var partlyOverlapping = await PostSessionAsync(adminClient, serviceTypeId, start.AddMinutes(30), capacity: 10);

        // Assert
        var created = await ReadAsync<AdminSessionResponse>(sameTime, HttpStatusCode.Created);
        Assert.Equal($"/api/v1/admin/sessions/{created.Id}", sameTime.Headers.Location?.OriginalString);
        Assert.Equal(HttpStatusCode.Created, partlyOverlapping.StatusCode);
    }

    [Fact]
    public async Task UpdateSession_CapacityBelowActiveBookings_ReturnsConflict()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddDays(1), capacity: 3);
        foreach (var _ in Enumerable.Range(0, 2))
        {
            using var user = CreateClient((await CreateUserAsync()).Token);
            using var booked = await BookAsync(user, session.Id);
            Assert.Equal(HttpStatusCode.Created, booked.StatusCode);
        }

        var request = new { startsAt = session.StartsAt, endsAt = session.EndsAt, capacity = 1, location = session.Location, notes = session.Notes };

        // Act
        using var response = await adminClient.PutAsJsonAsync($"/api/v1/admin/sessions/{session.Id}", request, Json, Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.SessionCapacityBelowActiveBookings);
        Assert.Equal(new SessionState(3, 2, 2), await GetSessionStateAsync(session.Id));
    }

    [Fact]
    public async Task UpdateSession_CapacityEqualToActiveBookings_Succeeds()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddDays(1), capacity: 3);
        using var user = CreateClient((await CreateUserAsync()).Token);
        using var booked = await BookAsync(user, session.Id);
        Assert.Equal(HttpStatusCode.Created, booked.StatusCode);
        var request = new { startsAt = session.StartsAt, endsAt = session.EndsAt, capacity = 1, location = session.Location, notes = session.Notes };

        // Act
        using var response = await adminClient.PutAsJsonAsync($"/api/v1/admin/sessions/{session.Id}", request, Json, Ct);

        // Assert
        var updated = await ReadAsync<AdminSessionResponse>(response, HttpStatusCode.OK);
        Assert.Equal(0, updated.AvailableSpots);
    }

    [Fact]
    public async Task DeleteSession_WithActiveBookings_ReturnsConflict()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddDays(1), capacity: 3);
        using var user = CreateClient((await CreateUserAsync()).Token);
        using var booked = await BookAsync(user, session.Id);
        Assert.Equal(HttpStatusCode.Created, booked.StatusCode);

        // Act
        using var response = await adminClient.DeleteAsync($"/api/v1/admin/sessions/{session.Id}", Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.SessionHasActiveBookings);
    }

    [Fact]
    public async Task CancelSession_WithBookings_CancelsSessionAndBookings()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddDays(1), capacity: 3);
        var user = await CreateUserAsync();
        using var userClient = CreateClient(user.Token);
        using var booked = await BookAsync(userClient, session.Id);
        Assert.Equal(HttpStatusCode.Created, booked.StatusCode);

        // Act
        using var response = await adminClient.PostAsync($"/api/v1/admin/sessions/{session.Id}/cancel", content: null, Ct);

        // Assert
        var cancelled = await ReadAsync<AdminSessionResponse>(response, HttpStatusCode.OK);
        Assert.True(cancelled.IsCancelled);
        Assert.Equal(0, await CountActiveBookingsAsync(user.Id));
    }
}
