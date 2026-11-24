using System.Net;
using System.Net.Http.Json;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceCategories;
using HjemIKroppenBooking.Application.Contracts.Responses.ServiceTypes;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.IntegrationTests.Infrastructure;

namespace HjemIKroppenBooking.IntegrationTests.ServiceCategories;

public sealed class AdminServiceCategoryTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateServiceType_InCategory_ReturnsCategorySummary()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var categoryId = await CreateCategoryAsync(adminClient);
        var request = new { categoryId, name = "Teatergruppe", description = (string?)null, defaultDurationMinutes = 90, defaultCapacity = 8, isActive = true };

        // Act
        using var response = await adminClient.PostAsJsonAsync("/api/v1/admin/service-types", request, Json, Ct);

        // Assert
        var serviceType = await ReadAsync<AdminServiceTypeResponse>(response, HttpStatusCode.Created);
        Assert.Equal(categoryId, serviceType.Category.Id);
        Assert.Equal(8, serviceType.DefaultCapacity);
    }

    [Fact]
    public async Task CreateCategory_DuplicateName_ReturnsConflict()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var name = $"Dans {Guid.NewGuid():N}";
        using var first = await adminClient.PostAsJsonAsync("/api/v1/admin/service-categories", new { name }, Json, Ct);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        // Act
        using var response = await adminClient.PostAsJsonAsync("/api/v1/admin/service-categories", new { name }, Json, Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.ServiceCategoryNameAlreadyExists);
    }

    [Fact]
    public async Task CreateCategory_NamesWithSameSlug_GetUniqueSlugs()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var key = Guid.NewGuid().ToString("N");

        // Act
        using var first = await adminClient.PostAsJsonAsync("/api/v1/admin/service-categories", new { name = $"Dans {key}!" }, Json, Ct);
        using var second = await adminClient.PostAsJsonAsync("/api/v1/admin/service-categories", new { name = $"Dans {key}?" }, Json, Ct);

        // Assert
        Assert.Equal($"dans-{key}", (await ReadAsync<AdminServiceCategoryResponse>(first, HttpStatusCode.Created)).Slug);
        Assert.Equal($"dans-{key}-2", (await ReadAsync<AdminServiceCategoryResponse>(second, HttpStatusCode.Created)).Slug);
    }

    [Fact]
    public async Task RenameCategory_Always_UpdatesSlug()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var categoryId = await CreateCategoryAsync(adminClient);
        var key = Guid.NewGuid().ToString("N");

        // Act
        using var response = await adminClient.PutAsJsonAsync($"/api/v1/admin/service-categories/{categoryId}", new { name = $"Bevægelse {key}" }, Json, Ct);

        // Assert
        var category = await ReadAsync<AdminServiceCategoryResponse>(response, HttpStatusCode.OK);
        Assert.Equal($"bevaegelse-{key}", category.Slug);
    }

    [Fact]
    public async Task DeleteCategory_WithServiceTypes_ReturnsConflict()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var serviceTypeId = await CreateGroupServiceTypeAsync(adminClient);
        using var serviceTypeResponse = await adminClient.GetAsync($"/api/v1/admin/service-types/{serviceTypeId}", Ct);
        var serviceType = await ReadAsync<AdminServiceTypeResponse>(serviceTypeResponse, HttpStatusCode.OK);

        // Act
        using var response = await adminClient.DeleteAsync($"/api/v1/admin/service-categories/{serviceType.Category.Id}", Ct);

        // Assert
        await AssertProblemAsync(response, HttpStatusCode.Conflict, ErrorCodes.ServiceCategoryInUse);
    }

    [Fact]
    public async Task DeleteCategory_Unused_ReturnsNoContent()
    {
        // Arrange
        var admin = await CreateAdminAsync();
        using var adminClient = CreateClient(admin.Token);
        var categoryId = await CreateCategoryAsync(adminClient);

        // Act
        using var response = await adminClient.DeleteAsync($"/api/v1/admin/service-categories/{categoryId}", Ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var getResponse = await adminClient.GetAsync($"/api/v1/admin/service-categories/{categoryId}", Ct);
        await AssertProblemAsync(getResponse, HttpStatusCode.NotFound, ErrorCodes.ServiceCategoryNotFound);
    }
}
