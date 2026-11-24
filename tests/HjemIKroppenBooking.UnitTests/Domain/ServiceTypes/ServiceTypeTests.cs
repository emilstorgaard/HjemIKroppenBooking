using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.ServiceTypes;
using HjemIKroppenBooking.Domain.Sessions;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Domain.ServiceTypes;

public sealed class ServiceTypeTests
{
    [Fact]
    public void Create_Valid_StoresTrimmedValues()
    {
        // Arrange
        const string name = "  Saturday group  ";

        // Act
        var serviceType = ServiceType.Create(Category(), name, "  ", 90, 12, isActive: true);

        // Assert
        Assert.Equal("Saturday group", serviceType.Name);
        Assert.Null(serviceType.Description);
        Assert.Equal(12, serviceType.DefaultCapacity);
        Assert.True(serviceType.IsActive);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(8)]
    public void Create_AnyCategory_AllowsOneOrMoreSpots(int capacity)
    {
        // Arrange
        var category = Category();

        // Act
        var serviceType = ServiceType.Create(category, "Talk therapy", null, 60, capacity, isActive: true);

        // Assert
        Assert.Equal(capacity, serviceType.DefaultCapacity);
    }

    [Theory]
    [InlineData(Session.MinDurationMinutes - 1)]
    [InlineData(Session.MaxDurationMinutes + 1)]
    public void Create_DefaultDurationOutOfRange_ThrowsDurationOutOfRange(int durationMinutes)
    {
        // Arrange
        const string name = "Dance therapy";

        // Act
        var exception = Record.Exception(() => ServiceType.Create(Category(), name, null, durationMinutes, 1, isActive: true));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationDurationOutOfRange, ErrorType.Validation);
    }

    [Fact]
    public void Create_NameTooLong_ThrowsMaxLength()
    {
        // Arrange
        var name = new string('x', ServiceType.NameMaxLength + 1);

        // Act
        var exception = Record.Exception(() => ServiceType.Create(Category(), name, null, 60, 1, isActive: true));

        // Assert
        var error = AssertDomainError(exception, ErrorCodes.ValidationMaxLength, ErrorType.Validation);
        Assert.Equal(ServiceType.NameMaxLength, error.Parameters["maxLength"]);
    }

    [Fact]
    public void Create_WithCategory_StoresCategory()
    {
        // Arrange
        var category = Category();

        // Act
        var serviceType = ServiceType.Create(category, "Dance therapy", null, 60, 1, isActive: true);

        // Assert
        Assert.Equal(category.Id, serviceType.CategoryId);
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
