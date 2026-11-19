using HjemIKroppenBooking.Application.Features.Users.ChangeUserRole;
using HjemIKroppenBooking.Application.Features.Users.Common;
using HjemIKroppenBooking.Application.Features.Users.ResetUserPassword;
using HjemIKroppenBooking.Application.Features.Users.UpdateMyPassword;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.Users;
using HjemIKroppenBooking.UnitTests.Application.Fakes;
using Microsoft.Extensions.Time.Testing;
using static HjemIKroppenBooking.UnitTests.Domain.TestData;

namespace HjemIKroppenBooking.UnitTests.Application.Users;

public sealed class UserHandlerTests
{
    private readonly InMemoryUserRepository _users = new();
    private readonly InMemorySessionRepository _sessions = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(Now));

    [Fact]
    public async Task Deactivate_UserWithBookings_CancelsOnlyNotStartedBookingsAndDeactivates()
    {
        // Arrange
        var user = AddUser("anna@example.dk");
        var started = CreateSession(startsAt: Now.AddHours(1));
        var upcoming = CreateSession(startsAt: Now.AddDays(2));
        var startedBooking = started.Book(user.Id, null, Now);
        var upcomingBooking = upcoming.Book(user.Id, null, Now);
        _sessions.Add(started);
        _sessions.Add(upcoming);
        _time.SetUtcNow(new DateTimeOffset(Now.AddHours(1).AddMinutes(5)));

        // Act
        var cancelled = await new UserDeactivator(_sessions, _unitOfWork, _time).DeactivateAsync(user, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(1, cancelled);
        Assert.False(user.IsActive);
        Assert.True(startedBooking.IsActive);
        Assert.False(upcomingBooking.IsActive);
        Assert.Equal(0, upcoming.ActiveBookingCount);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task ChangeRole_OwnAccount_ThrowsCannotChangeOwnAccount()
    {
        // Arrange
        var admin = AddUser("admin@example.dk");
        var handler = new ChangeUserRoleCommandHandler(new FakeCurrentUser(admin.Id), _users, _unitOfWork);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(new ChangeUserRoleCommand(admin.Id, UserRole.User), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.UserCannotChangeOwnAccount, ErrorType.Conflict);
    }

    [Fact]
    public async Task ChangeRole_OtherUser_ChangesRole()
    {
        // Arrange
        var user = AddUser("anna@example.dk");
        var handler = new ChangeUserRoleCommandHandler(new FakeCurrentUser(Guid.NewGuid()), _users, _unitOfWork);

        // Act
        var response = await handler.Handle(new ChangeUserRoleCommand(user.Id, UserRole.Admin), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(UserRole.Admin, response.Role);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task ResetPassword_OwnAccount_ThrowsCannotChangeOwnAccount()
    {
        // Arrange
        var admin = AddUser("admin@example.dk");
        var handler = new ResetUserPasswordCommandHandler(new FakeCurrentUser(admin.Id), _users, _hasher, _unitOfWork);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(new ResetUserPasswordCommand(admin.Id, "new password"), TestContext.Current.CancellationToken));

        // Assert
        AssertDomainError(exception, ErrorCodes.UserCannotChangeOwnAccount, ErrorType.Conflict);
    }

    [Fact]
    public async Task UpdateMyPassword_WrongCurrentPassword_ThrowsWrongPasswordForField()
    {
        // Arrange
        var user = AddUser("anna@example.dk");
        var handler = new UpdateMyPasswordCommandHandler(new FakeCurrentUser(user.Id), _users, _hasher, _unitOfWork);

        // Act
        var exception = await Record.ExceptionAsync(() => handler.Handle(new UpdateMyPasswordCommand("wrong", "new password"), TestContext.Current.CancellationToken));

        // Assert
        var error = AssertDomainError(exception, ErrorCodes.AuthWrongPassword, ErrorType.Validation);
        Assert.Equal("currentPassword", error.Parameters["field"]);
        Assert.Equal(0, _unitOfWork.SaveCount);
    }

    private User AddUser(string email)
    {
        var user = User.Register(email, "Name", FakePasswordHasher.Prefix + "current password");
        _users.Add(user);
        return user;
    }
}
