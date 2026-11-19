namespace HjemIKroppenBooking.Domain.ServiceTypes;

/// <summary>Main category of a <see cref="ServiceType"/>.</summary>
public enum ServiceCategory
{
    /// <summary>Individual dance therapy.</summary>
    Dance = 0,

    /// <summary>Individual talk therapy.</summary>
    Conversation = 1,

    /// <summary>Group therapy such as theatre, workshops and series.</summary>
    Group = 2
}
