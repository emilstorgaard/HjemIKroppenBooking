using System.Net;
using System.Net.Http.Json;
using System.Text;
using HjemIKroppenBooking.Api.ErrorHandling;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.IntegrationTests.Infrastructure;

namespace HjemIKroppenBooking.IntegrationTests.ErrorHandling;

public sealed class ErrorResponseTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Register_InvalidInput_ReturnsFieldErrorsWithCodesAndDanishMessages()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        using var response = await client.PostAsJsonAsync("/api/v1/auth/register", new { email = "not-an-email", fullName = "", password = "short" }, Json, Ct);

        // Assert
        var problem = await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, ErrorCodes.ValidationFailed);
        Assert.NotNull(problem.Errors);
        Assert.Equal(new FieldError(ErrorCodes.ValidationEmailInvalid, ErrorCatalog.GetMessage(ErrorCodes.ValidationEmailInvalid)), Assert.Single(problem.Errors["email"]));
        Assert.Equal(ErrorCodes.ValidationRequired, Assert.Single(problem.Errors["fullName"]).Code);
        Assert.Equal(ErrorCodes.ValidationPasswordTooShort, Assert.Single(problem.Errors["password"]).Code);
    }

    [Fact]
    public async Task Register_MissingFields_ReturnsRequiredErrors()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        using var response = await client.PostAsJsonAsync("/api/v1/auth/register", new { }, Json, Ct);

        // Assert
        var problem = await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, ErrorCodes.ValidationFailed);
        Assert.Equal(ErrorCodes.ValidationRequired, Assert.Single(problem.Errors!["email"]).Code);
        Assert.Equal(ErrorCodes.ValidationRequired, Assert.Single(problem.Errors["password"]).Code);
    }

    [Fact]
    public async Task Register_MalformedJson_ReturnsRequestInvalid()
    {
        // Arrange
        using var client = CreateClient();
        using var content = new StringContent("{ \"email\": ", Encoding.UTF8, "application/json");

        // Act
        using var response = await client.PostAsync("/api/v1/auth/register", content, Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.BadRequest, ErrorCodes.RequestInvalid);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsInvalidCredentials()
    {
        // Arrange
        var user = await CreateUserAsync();
        using var client = CreateClient();

        // Act
        using var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = user.Email, password = "Wrong-Password-999" }, Json, Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Unauthorized, ErrorCodes.AuthInvalidCredentials);
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsSameErrorAsWrongPassword()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        using var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = "nobody@example.dk", password = Password }, Json, Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Unauthorized, ErrorCodes.AuthInvalidCredentials);
    }

    [Fact]
    public async Task CreateSession_CapacityOutOfRange_ReturnsFieldError()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var serviceTypeId = await CreateGroupServiceTypeAsync(adminClient);

        // Act
        using var response = await PostSessionAsync(adminClient, serviceTypeId, Now.AddDays(1), capacity: 0);

        // Assert
        var problem = await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, ErrorCodes.ValidationFailed);
        Assert.Equal(ErrorCodes.ValidationCapacityOutOfRange, Assert.Single(problem.Errors!["capacity"]).Code);
    }

    [Fact]
    public async Task CreateSession_StartInPast_ReturnsFieldError()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var serviceTypeId = await CreateGroupServiceTypeAsync(adminClient);

        // Act
        using var response = await PostSessionAsync(adminClient, serviceTypeId, Now.AddHours(-1), capacity: 5);

        // Assert
        var problem = await AssertProblemAsync(response, HttpStatusCode.UnprocessableEntity, ErrorCodes.ValidationFailed);
        Assert.Equal(ErrorCodes.SessionStartInPast, Assert.Single(problem.Errors!["startsAt"]).Code);
    }

    [Theory]
    [InlineData("/api/v1/sessions/{0}", ErrorCodes.SessionNotFound)]
    [InlineData("/api/v1/series/{0}", ErrorCodes.SeriesNotFound)]
    [InlineData("/api/v1/service-types/{0}", ErrorCodes.ServiceTypeNotFound)]
    public async Task GetPublicResource_UnknownId_ReturnsNotFound(string path, string expectedCode)
    {
        // Arrange
        using var client = CreateClient();
        var url = string.Format(System.Globalization.CultureInfo.InvariantCulture, path, Guid.NewGuid());

        // Act
        using var response = await client.GetAsync(url, Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.NotFound, expectedCode);
    }

    [Fact]
    public async Task CancelMyBooking_UnknownId_ReturnsNotFound()
    {
        // Arrange
        using var client = CreateClient((await CreateUserAsync()).Token);

        // Act
        using var response = await client.DeleteAsync($"/api/v1/me/bookings/{Guid.NewGuid()}", Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.NotFound, ErrorCodes.BookingNotFound);
    }

    [Fact]
    public async Task BookSeries_UnknownSeries_ReturnsNotFound()
    {
        // Arrange
        using var client = CreateClient((await CreateUserAsync()).Token);

        // Act
        using var response = await client.PostAsJsonAsync($"/api/v1/me/series/{Guid.NewGuid()}/bookings", new { note = (string?)null }, Json, Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.NotFound, ErrorCodes.SeriesNotFound);
    }

    [Fact]
    public async Task UpdateSession_UnknownId_ReturnsNotFound()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var request = new { startsAt = Now.AddDays(1), endsAt = Now.AddDays(1).AddHours(1), capacity = 5, location = (string?)null, notes = (string?)null };

        // Act
        using var response = await adminClient.PutAsJsonAsync($"/api/v1/admin/sessions/{Guid.NewGuid()}", request, Json, Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.NotFound, ErrorCodes.SessionNotFound);
    }

    [Fact]
    public async Task UnknownRoute_ReturnsRequestNotFound()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        using var response = await client.GetAsync("/api/v1/does-not-exist", Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.NotFound, ErrorCodes.RequestNotFound);
    }

    [Fact]
    public async Task GetMe_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        using var client = CreateClient();

        // Act
        using var response = await client.GetAsync("/api/v1/me", Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Unauthorized, ErrorCodes.AuthUnauthorized);
    }

    [Fact]
    public async Task AdminEndpoint_AsNormalUser_ReturnsForbidden()
    {
        // Arrange
        using var client = CreateClient((await CreateUserAsync()).Token);

        // Act
        using var response = await client.GetAsync("/api/v1/admin/users", Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Forbidden, ErrorCodes.AuthForbidden);
    }

    [Fact]
    public async Task AnyRequest_WithCorrelationId_EchoesItInHeaderAndProblem()
    {
        // Arrange
        using var client = CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/sessions/{Guid.NewGuid()}");
        request.Headers.Add("X-Correlation-Id", "test-correlation-123");

        // Act
        using var response = await client.SendAsync(request, Ct);

        // Assert
        var problem = await AssertProblemAsync(response, HttpStatusCode.NotFound, ErrorCodes.SessionNotFound);
        Assert.Equal("test-correlation-123", problem.CorrelationId);
        Assert.Equal("test-correlation-123", Assert.Single(response.Headers.GetValues("X-Correlation-Id")));
        Assert.Equal("nosniff", Assert.Single(response.Headers.GetValues("X-Content-Type-Options")));
    }

    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task HealthEndpoint_Anonymous_ReturnsHealthy(string path)
    {
        // Arrange
        using var client = CreateClient();

        // Act
        using var response = await client.GetAsync(path, Ct);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync(Ct));
    }
}
