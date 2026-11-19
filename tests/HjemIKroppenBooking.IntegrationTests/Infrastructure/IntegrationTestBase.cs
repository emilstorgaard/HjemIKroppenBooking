using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Application.Contracts.Responses.Auth;
using HjemIKroppenBooking.Application.Contracts.Responses.Series;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Application.Contracts.Responses.Sessions;
using HjemIKroppenBooking.Application.Contracts.Responses.Users;
using HjemIKroppenBooking.Domain.Bookings;
using HjemIKroppenBooking.Domain.Users;
using HjemIKroppenBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HjemIKroppenBooking.IntegrationTests.Infrastructure;

/// <summary>
/// Base for integration tests: one <see cref="ApiFactory"/> (and database) per test class, plus helpers that set up
/// data through the public API, exactly as a client would.
/// </summary>
public abstract class IntegrationTestBase(ApiFactory factory) : IClassFixture<ApiFactory>
{
    protected const string Password = "Secure-Password-123";
    protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    protected ApiFactory Factory { get; } = factory;

    protected static CancellationToken Ct => TestContext.Current.CancellationToken;

    protected DateTime Now => Factory.Time.GetUtcNow().UtcDateTime;

    protected HttpClient CreateClient(string? token = null)
    {
        var client = Factory.CreateClient();
        if (token is not null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    protected async Task<TestUser> CreateUserAsync()
    {
        var email = $"user-{Guid.NewGuid():N}@example.dk";
        using var client = CreateClient();
        using var register = await client.PostAsJsonAsync("/api/v1/auth/register", new { email, fullName = "Test User", password = Password }, Json, Ct);
        var user = await ReadAsync<UserResponse>(register, HttpStatusCode.Created);
        return new TestUser(user.Id, email, await LoginAsync(email));
    }

    /// <summary>Registers a user and promotes it in the database, which is how the first admin is created in production too.</summary>
    protected async Task<TestUser> CreateAdminAsync()
    {
        var user = await CreateUserAsync();
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var entity = await db.Users.SingleAsync(u => u.Id == user.Id, Ct);
            entity.ChangeRole(UserRole.Admin);
            await db.SaveChangesAsync(Ct);
        }

        return user with { Token = await LoginAsync(user.Email) };
    }

    protected async Task<string> LoginAsync(string email)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = Password }, Json, Ct);
        return (await ReadAsync<LoginResponse>(response, HttpStatusCode.OK)).AccessToken;
    }

    protected static async Task<Guid> CreateGroupServiceTypeAsync(HttpClient admin)
    {
        var request = new { category = "Group", name = $"Group {Guid.NewGuid():N}", description = (string?)null, defaultDurationMinutes = 60, defaultCapacity = 10, sortOrder = 0, isActive = true };
        using var response = await admin.PostAsJsonAsync("/api/v1/admin/service-types", request, Json, Ct);
        return (await ReadAsync<AdminServiceTypeResponse>(response, HttpStatusCode.Created)).Id;
    }

    protected static async Task<AdminSessionResponse> CreateSessionAsync(HttpClient admin, Guid serviceTypeId, DateTime startsAt, int capacity)
    {
        using var response = await PostSessionAsync(admin, serviceTypeId, startsAt, capacity);
        return await ReadAsync<AdminSessionResponse>(response, HttpStatusCode.Created);
    }

    protected static Task<HttpResponseMessage> PostSessionAsync(HttpClient admin, Guid serviceTypeId, DateTime startsAt, int capacity)
    {
        var request = new { serviceTypeId, startsAt, durationMinutes = 60, capacity, location = "Studio", notes = (string?)null };
        return admin.PostAsJsonAsync("/api/v1/admin/sessions", request, Json, Ct);
    }

    protected static async Task<AdminSeriesResponse> CreateSeriesAsync(HttpClient admin, Guid serviceTypeId, IReadOnlyList<DateTime> startTimes, int capacity)
    {
        var request = new { serviceTypeId, title = "Test series", startTimes, durationMinutes = 60, capacity, location = "Studio", notes = (string?)null };
        using var response = await admin.PostAsJsonAsync("/api/v1/admin/series", request, Json, Ct);
        return await ReadAsync<AdminSeriesResponse>(response, HttpStatusCode.Created);
    }

    protected static Task<HttpResponseMessage> BookAsync(HttpClient user, Guid sessionId) =>
        user.PostAsJsonAsync("/api/v1/me/bookings", new { sessionId, note = (string?)null }, Json, Ct);

    protected static async Task<T> ReadAsync<T>(HttpResponseMessage response, HttpStatusCode expectedStatus)
    {
        ArgumentNullException.ThrowIfNull(response);
        var body = await response.Content.ReadAsStringAsync(Ct);
        if (response.StatusCode != expectedStatus)
        {
            Assert.Fail($"Expected {(int)expectedStatus}, got {(int)response.StatusCode}: {body}");
        }

        return JsonSerializer.Deserialize<T>(body, Json) ?? throw new InvalidOperationException("Empty response body.");
    }

    /// <summary>Asserts a problem response with the given status and code, and that its detail is the Danish catalog message.</summary>
    protected static async Task<ProblemResponse> AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)
    {
        var problem = await ReadAsync<ProblemResponse>(response, expectedStatus);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(expectedCode, problem.Code);
        Assert.Equal(ErrorCatalog.GetMessage(expectedCode), problem.Detail);
        Assert.False(string.IsNullOrEmpty(problem.TraceId));
        return problem;
    }

    protected async Task<SessionState> GetSessionStateAsync(Guid sessionId)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var session = await db.Sessions.AsNoTracking().Where(s => s.Id == sessionId).Select(s => new { s.Capacity, s.ActiveBookingCount }).SingleAsync(Ct);
        var rows = await db.Bookings.CountAsync(b => b.SessionId == sessionId && b.Status == BookingStatus.Active, Ct);
        return new SessionState(session.Capacity, session.ActiveBookingCount, rows);
    }

    protected async Task<int> CountActiveBookingsAsync(Guid userId)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Bookings.CountAsync(b => b.UserId == userId && b.Status == BookingStatus.Active, Ct);
    }
}
