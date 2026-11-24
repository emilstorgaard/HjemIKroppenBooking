using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.ServiceCategories;
using HjemIKroppenBooking.Domain.Sessions;

namespace HjemIKroppenBooking.Domain.ServiceTypes;

/// <summary>
/// Something that can be booked, e.g. dance therapy or a named group format. Concrete times are
/// <see cref="Session"/>s. Inactive service types are hidden from users but keep their history.
/// </summary>
public sealed class ServiceType : Entity
{
    public const int NameMaxLength = 150;
    public const int DescriptionMaxLength = 2000;

    private ServiceType()
    {
    }

    public Guid CategoryId { get; private set; }

    /// <summary>Navigation for queries. Repositories always include it.</summary>
    public ServiceCategory Category { get; private set; } = null!;

    /// <summary>Display name. Unique within a category.</summary>
    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    /// <summary>Duration used when an admin creates a session without specifying one.</summary>
    public int DefaultDurationMinutes { get; private set; }

    /// <summary>Capacity used when an admin creates a session without specifying one.</summary>
    public int DefaultCapacity { get; private set; }

    public bool IsActive { get; private set; }

    /// <summary>Display order. Lowest first.</summary>
    public int SortOrder { get; private set; }

    public static ServiceType Create(
        ServiceCategory category, string name, string? description, int defaultDurationMinutes, int defaultCapacity, int sortOrder, bool isActive)
    {
        var serviceType = new ServiceType();
        serviceType.Update(category, name, description, defaultDurationMinutes, defaultCapacity, sortOrder, isActive);
        return serviceType;
    }

    public void Update(
        ServiceCategory category, string name, string? description, int defaultDurationMinutes, int defaultCapacity, int sortOrder, bool isActive)
    {
        ArgumentNullException.ThrowIfNull(category);
        var validName = Guard.RequiredText(name, NameMaxLength, "name");
        var validDescription = Guard.OptionalText(description, DescriptionMaxLength, "description");
        Guard.InRange(defaultDurationMinutes, Session.MinDurationMinutes, Session.MaxDurationMinutes, ErrorCodes.ValidationDurationOutOfRange, "defaultDurationMinutes");
        Guard.InRange(defaultCapacity, Session.MinCapacity, Session.MaxCapacity, ErrorCodes.ValidationCapacityOutOfRange, "defaultCapacity");

        CategoryId = category.Id;
        Category = category;
        Name = validName;
        Description = validDescription;
        DefaultDurationMinutes = defaultDurationMinutes;
        DefaultCapacity = defaultCapacity;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
