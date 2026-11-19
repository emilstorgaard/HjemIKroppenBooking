using System.Net;
using System.Net.Http.Json;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.IntegrationTests.Infrastructure;

namespace HjemIKroppenBooking.IntegrationTests.Series;

public sealed class SeriesBookingTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task BookSeries_OneSessionFull_BooksNothingAndNamesTheFullSession()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var serviceTypeId = await CreateGroupServiceTypeAsync(adminClient);
        var series = await CreateSeriesAsync(adminClient, serviceTypeId, [Now.AddHours(2), Now.AddHours(4), Now.AddHours(6)], capacity: 1);
        var fullSession = series.Sessions[1];
        using var other = CreateClient((await CreateUserAsync()).Token);
        using var otherBooking = await BookAsync(other, fullSession.Id);
        Assert.Equal(HttpStatusCode.Created, otherBooking.StatusCode);
        var user = await CreateUserAsync();
        using var client = CreateClient(user.Token);

        // Act
        using var response = await client.PostAsJsonAsync($"/api/v1/me/series/{series.Id}/bookings", new { note = (string?)null }, Json, Ct);

        // Assert
        var problem = await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.SessionFull);
        Assert.Equal(fullSession.Id, problem.Parameters!["sessionId"].GetGuid());
        Assert.Equal(0, await CountActiveBookingsAsync(user.Id));
        Assert.Equal(0, (await GetSessionStateAsync(series.Sessions[0].Id)).ActiveBookingCount);
        Assert.Equal(0, (await GetSessionStateAsync(series.Sessions[2].Id)).ActiveBookingCount);
    }

    [Fact]
    public async Task BookSeries_OneSessionAlreadyBooked_BooksOnlyTheOthers()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var serviceTypeId = await CreateGroupServiceTypeAsync(adminClient);
        var series = await CreateSeriesAsync(adminClient, serviceTypeId, [Now.AddHours(2), Now.AddHours(4), Now.AddHours(6)], capacity: 5);
        var user = await CreateUserAsync();
        using var client = CreateClient(user.Token);
        using var single = await BookAsync(client, series.Sessions[0].Id);
        Assert.Equal(HttpStatusCode.Created, single.StatusCode);

        // Act
        using var response = await client.PostAsJsonAsync($"/api/v1/me/series/{series.Id}/bookings", new { note = "Looking forward to it" }, Json, Ct);

        // Assert
        var result = await ReadAsync<SeriesBookingResponse>(response, HttpStatusCode.Created);
        Assert.Equal(2, result.Bookings.Count);
        Assert.DoesNotContain(result.Bookings, b => b.SessionId == series.Sessions[0].Id);
        Assert.Equal(3, await CountActiveBookingsAsync(user.Id));
    }

    [Fact]
    public async Task CancelSeriesBooking_AfterFirstSessionStarted_CancelsOnlyFutureSessions()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var serviceTypeId = await CreateGroupServiceTypeAsync(adminClient);
        var series = await CreateSeriesAsync(adminClient, serviceTypeId, [Now.AddHours(1), Now.AddHours(3), Now.AddHours(5)], capacity: 5);
        var user = await CreateUserAsync();
        using var client = CreateClient(user.Token);
        using var booked = await client.PostAsJsonAsync($"/api/v1/me/series/{series.Id}/bookings", new { note = "Looking forward to it" }, Json, Ct);
        Assert.Equal(HttpStatusCode.Created, booked.StatusCode);
        Factory.Time.SetUtcNow(new DateTimeOffset(series.Sessions[0].StartsAt.AddMinutes(10), TimeSpan.Zero));

        // Act
        using var response = await client.DeleteAsync($"/api/v1/me/series/{series.Id}/bookings", Ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(1, (await GetSessionStateAsync(series.Sessions[0].Id)).ActiveBookingCount);
        Assert.Equal(0, (await GetSessionStateAsync(series.Sessions[1].Id)).ActiveBookingCount);
        Assert.Equal(0, (await GetSessionStateAsync(series.Sessions[2].Id)).ActiveBookingCount);
    }
}
