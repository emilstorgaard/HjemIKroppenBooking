using FluentValidation.Results;
using HjemIKroppenBooking.Application.Features.Auth.Register;
using HjemIKroppenBooking.Application.Features.Series.CreateSeries;
using HjemIKroppenBooking.Application.Features.ServiceTypes.CreateServiceType;
using HjemIKroppenBooking.Application.Features.Sessions.ListSessions;
using HjemIKroppenBooking.Application.Features.Sessions.UpdateSession;
using HjemIKroppenBooking.Application.Features.Users.UpdateMyPassword;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Series;
using HjemIKroppenBooking.Domain.ServiceTypes;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Application.Validation;

public sealed class ValidatorTests
{
    [Fact]
    public void RegisterValidator_ValidCommand_HasNoErrors()
    {
        // Arrange
        var command = new RegisterCommand("anna@example.dk", "Anna Hansen", "correct horse");

        // Act
        var result = new RegisterCommandValidator().Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("anna@localhost")]
    [InlineData("Anna <anna@example.dk>")]
    public void RegisterValidator_InvalidEmail_ReturnsEmailInvalid(string email)
    {
        // Arrange
        var command = new RegisterCommand(email, "Anna", "correct horse");

        // Act
        var result = new RegisterCommandValidator().Validate(command);

        // Assert
        AssertSingleError(result, "Email", ErrorCodes.ValidationEmailInvalid);
    }

    [Fact]
    public void RegisterValidator_PasswordTooShort_ReturnsPasswordTooShort()
    {
        // Arrange
        var command = new RegisterCommand("anna@example.dk", "Anna", "short");

        // Act
        var result = new RegisterCommandValidator().Validate(command);

        // Assert
        AssertSingleError(result, "Password", ErrorCodes.ValidationPasswordTooShort);
    }

    [Fact]
    public void RegisterValidator_PasswordOver72Bytes_ReturnsPasswordTooLong()
    {
        // Arrange: 40 characters, but 80 bytes in UTF-8 because each "æ" takes two bytes.
        var command = new RegisterCommand("anna@example.dk", "Anna", new string('æ', 40));

        // Act
        var result = new RegisterCommandValidator().Validate(command);

        // Assert
        AssertSingleError(result, "Password", ErrorCodes.ValidationPasswordTooLong);
    }

    [Fact]
    public void UpdateMyPasswordValidator_NewEqualsCurrent_ReturnsPasswordUnchanged()
    {
        // Arrange
        var command = new UpdateMyPasswordCommand("same password", "same password");

        // Act
        var result = new UpdateMyPasswordCommandValidator().Validate(command);

        // Assert
        AssertSingleError(result, "NewPassword", ErrorCodes.ValidationPasswordUnchanged);
    }

    [Fact]
    public void UpdateSessionValidator_EndBeforeStart_ReturnsEndBeforeStart()
    {
        // Arrange
        var command = new UpdateSessionCommand(Guid.NewGuid(), Now.AddDays(1), Now.AddDays(1).AddMinutes(-10), 5, null, null);

        // Act
        var result = new UpdateSessionCommandValidator().Validate(command);

        // Assert
        AssertSingleError(result, "EndsAt", ErrorCodes.ValidationEndBeforeStart);
    }

    [Fact]
    public void CreateSeriesValidator_TooManyStartTimes_ReturnsTooManyItems()
    {
        // Arrange
        var starts = Enumerable.Range(1, SessionSeries.MaxSessions + 1).Select(i => Now.AddDays(i)).ToList();
        var command = new CreateSeriesCommand(Guid.NewGuid(), null, starts, null, null, null, null);

        // Act
        var result = new CreateSeriesCommandValidator().Validate(command);

        // Assert
        AssertSingleError(result, "StartTimes", ErrorCodes.ValidationTooManyItems);
    }

    [Fact]
    public void CreateSeriesValidator_OverlappingStartTimes_AreAllowed()
    {
        // Arrange
        var start = Now.AddDays(3);
        var command = new CreateSeriesCommand(Guid.NewGuid(), null, [start, start, start.AddMinutes(15)], 60, 10, null, null);

        // Act
        var result = new CreateSeriesCommandValidator().Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0, 20, "Page", ErrorCodes.ValidationPageOutOfRange)]
    [InlineData(1, 0, "PageSize", ErrorCodes.ValidationPageSizeOutOfRange)]
    [InlineData(1, 101, "PageSize", ErrorCodes.ValidationPageSizeOutOfRange)]
    public void ListSessionsValidator_InvalidPaging_ReturnsPagingError(int page, int pageSize, string property, string expectedCode)
    {
        // Arrange
        var query = new ListSessionsQuery(Page: page, PageSize: pageSize);

        // Act
        var result = new ListSessionsQueryValidator().Validate(query);

        // Assert
        AssertSingleError(result, property, expectedCode);
    }

    [Fact]
    public void ListSessionsValidator_ToBeforeFrom_ReturnsDateRangeInvalid()
    {
        // Arrange
        var query = new ListSessionsQuery(From: Now, To: Now.AddDays(-1));

        // Act
        var result = new ListSessionsQueryValidator().Validate(query);

        // Assert
        AssertSingleError(result, "To", ErrorCodes.ValidationDateRangeInvalid);
    }

    [Theory]
    [InlineData(ServiceCategory.Dance, 2, false)]
    [InlineData(ServiceCategory.Conversation, 1, true)]
    [InlineData(ServiceCategory.Group, 12, true)]
    public void CreateServiceTypeValidator_Capacity_DependsOnCategory(ServiceCategory category, int capacity, bool expectedValid)
    {
        // Arrange
        var command = new CreateServiceTypeCommand(category, "Name", null, 60, capacity, 0, IsActive: true);

        // Act
        var result = new CreateServiceTypeCommandValidator().Validate(command);

        // Assert
        Assert.Equal(expectedValid, result.IsValid);
        Assert.All(result.Errors, e => Assert.Equal(ErrorCodes.ValidationIndividualCapacityMustBeOne, e.ErrorCode));
    }

    private static void AssertSingleError(ValidationResult result, string propertyName, string expectedCode)
    {
        var failure = Assert.Single(result.Errors);
        Assert.Equal(propertyName, failure.PropertyName);
        Assert.Equal(expectedCode, failure.ErrorCode);
    }
}
