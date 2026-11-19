using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Users;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Domain.Users;

public sealed class UserTests
{
    private const string Hash = "$2a$12$abcdefghijklmnopqrstuuJ1ZtYQ9b1V9Zb3Zb3Zb3Zb3Zb3Zb3Zb3";

    [Fact]
    public void Register_ValidInput_CreatesActiveUserWithNormalizedEmail()
    {
        // Arrange
        const string email = "  Anna.Hansen@Example.DK ";

        // Act
        var user = User.Register(email, "  Anna Hansen ", Hash);

        // Assert
        Assert.Equal("anna.hansen@example.dk", user.Email);
        Assert.Equal("Anna Hansen", user.FullName);
        Assert.Equal(UserRole.User, user.Role);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Register_BlankName_ThrowsRequiredForFullName()
    {
        // Arrange
        const string fullName = "   ";

        // Act
        var exception = Record.Exception(() => User.Register("anna@example.dk", fullName, Hash));

        // Assert
        var error = AssertDomainError(exception, ErrorCodes.ValidationRequired, ErrorType.Validation);
        Assert.Equal("fullName", error.Parameters["field"]);
    }

    [Fact]
    public void Register_EmailTooLong_ThrowsMaxLength()
    {
        // Arrange
        var email = new string('a', User.EmailMaxLength) + "@example.dk";

        // Act
        var exception = Record.Exception(() => User.Register(email, "Anna", Hash));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationMaxLength, ErrorType.Validation);
    }

    [Fact]
    public void ChangeRole_UndefinedRole_ThrowsEnumInvalidAndKeepsRole()
    {
        // Arrange
        var user = User.Register("anna@example.dk", "Anna", Hash);

        // Act
        var exception = Record.Exception(() => user.ChangeRole((UserRole)42));

        // Assert
        AssertDomainError(exception, ErrorCodes.ValidationEnumInvalid, ErrorType.Validation);
        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public void ChangeRole_Admin_SetsRole()
    {
        // Arrange
        var user = User.Register("anna@example.dk", "Anna", Hash);

        // Act
        user.ChangeRole(UserRole.Admin);

        // Assert
        Assert.Equal(UserRole.Admin, user.Role);
    }

    [Fact]
    public void Deactivate_ActiveUser_MakesUserInactive()
    {
        // Arrange
        var user = User.Register("anna@example.dk", "Anna", Hash);

        // Act
        user.Deactivate();

        // Assert
        Assert.False(user.IsActive);
    }
}
