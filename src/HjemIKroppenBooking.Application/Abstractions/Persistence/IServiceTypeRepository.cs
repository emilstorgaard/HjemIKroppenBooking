using HjemIKroppenBooking.Domain.ServiceTypes;

namespace HjemIKroppenBooking.Application.Abstractions.Persistence;

/// <summary>Loads and stores the service type aggregate. Returned entities are tracked.</summary>
public interface IServiceTypeRepository
{
    Task<ServiceType?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(ServiceCategory category, string name, Guid? excludeId, CancellationToken cancellationToken);

    /// <summary>True if any session or series uses the service type.</summary>
    Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken);

    void Add(ServiceType serviceType);

    void Remove(ServiceType serviceType);
}
