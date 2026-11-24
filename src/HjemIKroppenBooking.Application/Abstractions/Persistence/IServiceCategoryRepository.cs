using HjemIKroppenBooking.Domain.ServiceCategories;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Loads and stores the service category aggregate. Returned entities are tracked.</summary>
public interface IServiceCategoryRepository
{
    Task<ServiceCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken cancellationToken);

    /// <summary>True if any service type, active or not, belongs to the category.</summary>
    Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken);

    void Add(ServiceCategory category);

    void Remove(ServiceCategory category);
}
