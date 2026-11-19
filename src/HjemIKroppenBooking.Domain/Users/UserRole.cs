namespace HjemIKroppenBooking.Domain.Users;

/// <summary>Determines what a user is allowed to do.</summary>
public enum UserRole
{
    /// <summary>Can book and cancel their own bookings.</summary>
    User = 0,

    /// <summary>Can additionally manage service types, sessions, series, bookings and users.</summary>
    Admin = 1
}
