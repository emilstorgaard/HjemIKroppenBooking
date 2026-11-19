using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.ServiceTypes;
using HjemIKroppenBooking.Domain.Sessions;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Domain.ServiceTypes;

public sealed class ServiceTypeTests
{
    [Fact]
    public void Create_ValidGroup_StoresTrimmedValues()
    {
        // Arrange
        const string name = "  Saturday group  ";

        // Act
        var serviceType = ServiceType.Create(ServiceCategory.Group, name, "  ", 90, 12, 3, isActive: true);

        // Assert
        Assert.Equal("Saturday group", serviceType.Name);
        Assert.Null(serviceType.Description);
        Assert.Equal(12, serviceType.DefaultCapacity);
        Assert.True(serviceType.IsGroup);
        Assert.True(serviceType.IsActive);
    }

    [Fact]
    public void Create_IndividualWithCapacityAboveOne_ThrowsIndividualCapacityMustBeOne()
    {
        // Arrange
        const int capacity = 2;

        // Act
        var exception = Record.Exception(() => ServiceType.Create(ServiceCategory.Conversation, "Talk therapy", null, 60, capacity, 0, isActive: true));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationIndividualCapacityMustBeOne, ErrorType.Validation);
    }

    [Theory]
    [InlineData(Session.MinDurationMinutes - 1)]
    [InlineData(Session.MaxDurationMinutes + 1)]
    public void Create_DefaultDurationOutOfRange_ThrowsDurationOutOfRange(int durationMinutes)
    {
        // Arrange
        const string name = "Dance therapy";

        // Act
        var exception = Record.Exception(() => ServiceType.Create(ServiceCategory.Dance, name, null, durationMinutes, 1, 0, isActive: true));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationDurationOutOfRange, ErrorType.Validation);
    }

    [Fact]
    public void Create_NameTooLong_ThrowsMaxLength()
    {
        // Arrange
        var name = new string('x', ServiceType.NameMaxLength + 1);

        // Act
        var exception = Record.Exception(() => ServiceType.Create(ServiceCategory.Dance, name, null, 60, 1, 0, isActive: true));

        // Assert
        var error = AssertDomainError(exception, ErrorCodes.ValidationMaxLength, ErrorType.Validation);
        Assert.Equal(ServiceType.NameMaxLength, error.Parameters["maxLength"]);
    }

    [Fact]
    public void Create_UndefinedCategory_ThrowsEnumInvalid()
    {
        // Arrange
        const ServiceCategory category = (ServiceCategory)99;

        // Act
        var exception = Record.Exception(() => ServiceType.Create(category, "Unknown", null, 60, 1, 0, isActive: true));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationEnumInvalid, ErrorType.Validation);
    }

    [Fact]
    public void Deactivate_ActiveServiceType_MakesItInactive()
    {
        // Arrange
        var serviceType = IndividualServiceType();

        // Act
        serviceType.Deactivate();

        // Assert
        Assert.False(serviceType.IsActive);
    }
}
