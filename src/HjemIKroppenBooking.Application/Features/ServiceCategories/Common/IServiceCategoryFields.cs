namespace HjemIKroppenBooking.Application.Features.ServiceCategories.Common;

/// <summary>The editable fields shared by the create and update commands, so both use the same rules.</summary>
public interface IServiceCategoryFields
{
    string Name { get; }
}
