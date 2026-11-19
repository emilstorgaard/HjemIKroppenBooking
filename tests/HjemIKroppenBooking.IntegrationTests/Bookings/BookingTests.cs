using System.Net;
using System.Net.Http.Json;
using HjemIKroppenBooking.Application.Contracts.Responses.Bookings;
using HjemIKroppenBooking.Application.Contracts.Responses.Common;
using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.IntegrationTests.Infrastructure;

namespace HjemIKroppenBooking.IntegrationTests.Bookings;

public sealed class BookingTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Book_SessionAtCapacity_ReturnsSessionFull()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var serviceTypeId = await CreateGroupServiceTypeAsync(adminClient);
        var session = await CreateSessionAsync(adminClient, serviceTypeId, Now.AddHours(3), capacity: 1);
        using var first = CreateClient((await CreateUserAsync()).Token);
        using var second = CreateClient((await CreateUserAsync()).Token);
        using var firstResponse = await BookAsync(first, session.Id);
        await ReadAsync<BookingResponse>(firstResponse, HttpStatusCode.Created);

        // Act
        using var response = await BookAsync(second, session.Id);

        // Assert
        var problem = await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.SessionFull);
        Assert.Equal(session.Id, problem.Parameters!["sessionId"].GetGuid());
        Assert.Equal(new SessionState(1, 1, 1), await GetSessionStateAsync(session.Id));
    }

    [Fact]
    public async Task Book_TwoOverlappingSessions_BothSucceed()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var serviceTypeId = await CreateGroupServiceTypeAsync(adminClient);
        var start = Now.AddHours(5);
        var first = await CreateSessionAsync(adminClient, serviceTypeId, start, capacity: 5);
        var overlapping = await CreateSessionAsync(adminClient, serviceTypeId, start.AddMinutes(30), capacity: 5);
        var user = await CreateUserAsync();
        using var client = CreateClient(user.Token);

        // Act
        using var firstResponse = await BookAsync(client, first.Id);
        using var secondResponse = await BookAsync(client, overlapping.Id);

        // Assert
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, secondResponse.StatusCode);
        Assert.Equal(2, await CountActiveBookingsAsync(user.Id));
    }

    [Fact]
    public async Task Book_SameSessionTwice_ReturnsBookingAlreadyExists()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddHours(3), capacity: 5);
        using var client = CreateClient((await CreateUserAsync()).Token);
        using var firstResponse = await BookAsync(client, session.Id);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        // Act
        using var response = await BookAsync(client, session.Id);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.BookingAlreadyExists);
        Assert.Equal(1, (await GetSessionStateAsync(session.Id)).ActiveBookingCount);
    }

    [Fact]
    public async Task Book_SessionThatHasStarted_ReturnsSessionAlreadyStarted()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddHours(1), capacity: 5);
        using var client = CreateClient((await CreateUserAsync()).Token);
        Factory.Time.SetUtcNow(new DateTimeOffset(session.StartsAt.AddMinutes(1), TimeSpan.Zero));

        // Act
        using var response = await BookAsync(client, session.Id);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.SessionAlreadyStarted);
    }

    [Fact]
    public async Task Cancel_JustBeforeStart_Succeeds()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddHours(2), capacity: 5);
        using var client = CreateClient((await CreateUserAsync()).Token);
        using var bookResponse = await BookAsync(client, session.Id);
        var booking = await ReadAsync<BookingResponse>(bookResponse, HttpStatusCode.Created);
        Factory.Time.SetUtcNow(new DateTimeOffset(session.StartsAt.AddSeconds(-1), TimeSpan.Zero));

        // Act
        using var response = await client.DeleteAsync($"/api/v1/me/bookings/{booking.Id}", Ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var getResponse = await client.GetAsync($"/api/v1/me/bookings/{booking.Id}", Ct);
        Assert.Equal(BookingStatus.Cancelled, (await ReadAsync<BookingResponse>(getResponse, HttpStatusCode.OK)).Status);
        Assert.Equal(new SessionState(5, 0, 0), await GetSessionStateAsync(session.Id));
    }

    [Fact]
    public async Task Cancel_AfterStart_ReturnsCancellationClosed()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddHours(2), capacity: 5);
        using var client = CreateClient((await CreateUserAsync()).Token);
        using var bookResponse = await BookAsync(client, session.Id);
        var booking = await ReadAsync<BookingResponse>(bookResponse, HttpStatusCode.Created);
        Factory.Time.SetUtcNow(new DateTimeOffset(session.StartsAt, TimeSpan.Zero));

        // Act
        using var response = await client.DeleteAsync($"/api/v1/me/bookings/{booking.Id}", Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.BookingCancellationClosed);
        Assert.Equal(1, (await GetSessionStateAsync(session.Id)).ActiveBookingCount);
    }

    [Fact]
    public async Task ListMyBookings_AfterBooking_ReturnsPagedBooking()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddHours(3), capacity: 5);
        using var client = CreateClient((await CreateUserAsync()).Token);
        using var bookResponse = await BookAsync(client, session.Id);
        Assert.Equal(HttpStatusCode.Created, bookResponse.StatusCode);

        // Act
        var page = await client.GetFromJsonAsync<PagedResponse<BookingResponse>>("/api/v1/me/bookings?page=1&pageSize=10", Json, Ct);

        // Assert
        Assert.NotNull(page);
        Assert.Equal(1, page.TotalCount);
        Assert.Equal(session.Id, Assert.Single(page.Items).SessionId);
    }
}
