namespace HjemIKroppenBooking.Application.Features.ServiceTypes.Common;

/// <summary>The editable fields shared by the create and update commands, so both use the same rules.</summary>
public interface IServiceTypeFields
{
    Guid CategoryId { get; }

    string Name { get; }

    string? Description { get; }

    int DefaultDurationMinutes { get; }

    int DefaultCapacity { get; }

    int SortOrder { get; }

    bool IsActive { get; }
}
