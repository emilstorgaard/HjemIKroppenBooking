using HjemIKroppenBooking.Domain.Common;

namespace HjemIKroppenBooking.Domain.Users;

/// <summary>
/// A person who can sign in. Deactivated users keep their history but can no longer sign in or be booked.
/// </summary>
public sealed class User : Entity
{
    public const int EmailMaxLength = 256;
    public const int FullNameMaxLength = 200;
    public const int PasswordHashMaxLength = 500;

    private User()
    {
    }

    /// <summary>Sign-in email, stored trimmed and in lower case. Unique.</summary>
    public string Email { get; private set; } = string.Empty;

    public string FullName { get; private set; } = string.Empty;

    /// <summary>Hash of the password. The password itself is never stored.</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }

    public bool IsActive { get; private set; }

    /// <summary>Creates an active user with the <see cref="UserRole.User"/> role.</summary>
    public static User Register(string email, string fullName, string passwordHash)
    {
        var user = new User { Role = UserRole.User, IsActive = true };
        user.UpdateProfile(fullName, email);
        user.ChangePasswordHash(passwordHash);
        return user;
    }

    /// <summary>The single place that defines how emails are compared and stored.</summary>
    public static string NormalizeEmail(string email) =>
        Guard.RequiredText(email, EmailMaxLength, "email").ToLowerInvariant();

    public void UpdateProfile(string fullName, string email)
    {
        var normalizedName = Guard.RequiredText(fullName, FullNameMaxLength, "fullName");
        var normalizedEmail = NormalizeEmail(email);
        FullName = normalizedName;
        Email = normalizedEmail;
    }

    public void ChangePasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
    }

    public void ChangeRole(UserRole role)
    {
        if (!Enum.IsDefined(role))
        {
            throw DomainException.Validation(ErrorCodes.ValidationEnumInvalid, Guard.Field("role"));
        }

        Role = role;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
