using System.Net;
using System.Net.Http.Json;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.IntegrationTests.Infrastructure;

namespace HjemIKroppenBooking.IntegrationTests.Concurrency;

/// <summary>
/// Races against the real database. Whatever the interleaving, the invariants must hold: never more active bookings than
/// capacity, and the denormalized counter always equals the number of active booking rows.
/// </summary>
public sealed class ConcurrencyTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    private const int Contenders = 6;
    private static readonly string[] BookingConflictCodes = [ErrorCodes.SessionFull, ErrorCodes.SessionConcurrencyConflict];
    private static readonly string[] UpdateConflictCodes = [ErrorCodes.SessionCapacityBelowActiveBookings, ErrorCodes.SessionConcurrencyConflict];

    [Fact]
    public async Task Book_ManyUsersRaceForLastSpot_ExactlyOneSucceeds()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddDays(1), capacity: 1);
        var clients = new List<HttpClient>();
        for (var i = 0; i < Contenders; i++)
        {
            clients.Add(CreateClient((await CreateUserAsync()).Token));
        }

        try
        {
            // Act
            var responses = await Task.WhenAll(clients.Select(client => BookAsync(client, session.Id)));

            // Assert
            try
            {
                Assert.Single(responses, r => r.StatusCode == HttpStatusCode.Created);
                foreach (var rejected in responses.Where(r => r.StatusCode != HttpStatusCode.Created))
                {
                    var problem = await ReadAsync<ProblemResponse>(rejected, HttpStatusCode.Conflict);
                    Assert.Contains(problem.Code, BookingConflictCodes);
                }

                Assert.Equal(new SessionState(1, 1, 1), await GetSessionStateAsync(session.Id));
            }
            finally
            {
                foreach (var response in responses)
                {
                    response.Dispose();
                }
            }
        }
        finally
        {
            foreach (var client in clients)
            {
                client.Dispose();
            }
        }
    }

    [Fact]
    public async Task UpdateCapacity_RacingWithBooking_InvariantHolds()
    {
        // Arrange: capacity 2 with one booking; then reduce to 1 while a second user books the last spot.
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var session = await CreateSessionAsync(adminClient, await CreateGroupServiceTypeAsync(adminClient), Now.AddDays(1), capacity: 2);
        using var firstUser = CreateClient((await CreateUserAsync()).Token);
        using var firstBooking = await BookAsync(firstUser, session.Id);
        Assert.Equal(HttpStatusCode.Created, firstBooking.StatusCode);
        using var secondUser = CreateClient((await CreateUserAsync()).Token);
        var update = new { startsAt = session.StartsAt, endsAt = session.EndsAt, capacity = 1, location = session.Location, notes = session.Notes };

        // Act
        var updateTask = adminClient.PutAsJsonAsync($"/api/v1/admin/sessions/{session.Id}", update, Json, Ct);
        var bookTask = BookAsync(secondUser, session.Id);
        using var updateResponse = await updateTask;
        using var bookResponse = await bookTask;

        // Assert: exactly one of the two wins, and the loser gets the matching conflict.
        var updated = updateResponse.StatusCode == HttpStatusCode.OK;
        var booked = bookResponse.StatusCode == HttpStatusCode.Created;
        Assert.True(updated ^ booked, $"Expected exactly one winner, got update {(int)updateResponse.StatusCode} and booking {(int)bookResponse.StatusCode}.");
        if (updated)
        {
            var problem = await ReadAsync<ProblemResponse>(bookResponse, HttpStatusCode.Conflict);
            Assert.Contains(problem.Code, BookingConflictCodes);
        }
        else
        {
            var problem = await ReadAsync<ProblemResponse>(updateResponse, HttpStatusCode.Conflict);
            Assert.Contains(problem.Code, UpdateConflictCodes);
        }

        var state = await GetSessionStateAsync(session.Id);
        Assert.InRange(state.ActiveBookingCount, 0, state.Capacity);
        Assert.Equal(state.ActiveBookingRows, state.ActiveBookingCount);
    }
}
