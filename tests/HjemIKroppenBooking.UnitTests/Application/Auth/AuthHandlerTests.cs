using HjemIKroppenBooking.Application.Features.Auth.Login;
using HjemIKroppenBooking.Application.Features.Auth.Register;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Users;
using HjemIKroppenBooking.UnitTests.Application.Fakes;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Application.Auth;

public sealed class AuthHandlerTests
{
    private const string Password = "correct horse";

    private readonly InMemoryUserRepository _users = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokenAndProfile()
    {
        // Arrange
        var user = AddUser("anna@example.dk");
        var handler = CreateLoginHandler();

        // Act
        var response = await handler.Handle(new LoginCommand(" Anna@Example.DK ", Password), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("token-for-anna@example.dk", response.AccessToken);
        Assert.Equal(user.Id, response.User.Id);
    }

    [Fact]
    public async Task Login_UnknownEmail_VerifiesDummyHashAndThrowsInvalidCredentials()
    {
        // Arrange
        var handler = CreateLoginHandler();

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(new LoginCommand("nobody@example.dk", Password), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.AuthInvalidCredentials, ErrorType.Unauthorized);
        Assert.Equal(_hasher.DummyHash, Assert.Single(_hasher.VerifiedHashes));
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsSameErrorAsUnknownEmail()
    {
        // Arrange
        AddUser("anna@example.dk");
        var handler = CreateLoginHandler();

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(new LoginCommand("anna@example.dk", "wrong password"), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.AuthInvalidCredentials, ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Login_DeactivatedUser_ThrowsInvalidCredentials()
    {
        // Arrange
        AddUser("anna@example.dk").Deactivate();
        var handler = CreateLoginHandler();

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(new LoginCommand("anna@example.dk", Password), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.AuthInvalidCredentials, ErrorType.Unauthorized);
    }

    [Fact]
    public async Task Register_NewEmail_AddsUserWithHashedPasswordAndUserRole()
    {
        // Arrange
        var handler = new RegisterCommandHandler(_users, _hasher, _unitOfWork);

        // Act
        var response = await handler.Handle(new RegisterCommand("New@Example.dk", "New User", Password), TestContext.Current.CancellationToken);

        // Assert
        var user = Assert.Single(_users.Users);
        Assert.Equal("new@example.dk", user.Email);
        Assert.Equal(FakePasswordHasher.Prefix + Password, user.PasswordHash);
        Assert.Equal(UserRole.User, response.Role);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task Register_ExistingEmailInOtherCase_ThrowsEmailAlreadyExists()
    {
        // Arrange
        AddUser("anna@example.dk");
        var handler = new RegisterCommandHandler(_users, _hasher, _unitOfWork);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(new RegisterCommand("ANNA@example.dk", "Anna", Password), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.UserEmailAlreadyExists, ErrorType.Conflict);
        Assert.Equal(0, _unitOfWork.SaveCount);
    }

    private LoginCommandHandler CreateLoginHandler() => new(_users, _hasher, new FakeJwtTokenService(Now.AddHours(12)));

    private User AddUser(string email)
    {
        var user = User.Register(email, "Anna", FakePasswordHasher.Prefix + Password);
        _users.Add(user);
        return user;
    }
}
