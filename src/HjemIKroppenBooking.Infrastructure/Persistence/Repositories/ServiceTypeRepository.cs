using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.ServiceTypes;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Repositories;

internal sealed class ServiceTypeRepository(AppDbContext db) : IServiceTypeRepository
{
    public Task<ServiceType?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.ServiceTypes.Include(s => s.Category).SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<bool> NameExistsAsync(Guid categoryId, string name, Guid? excludeId, CancellationToken cancellationToken) =>
        db.ServiceTypes.AnyAsync(s => s.CategoryId == categoryId && s.Name == name && (excludeId == null || s.Id != excludeId), cancellationToken);

    public async Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken) =>
        await db.Sessions.AnyAsync(s => s.ServiceTypeId == id, cancellationToken)
        || await db.SessionSeries.AnyAsync(s => s.ServiceTypeId == id, cancellationToken);

    public void Add(ServiceType serviceType) => db.ServiceTypes.Add(serviceType);

    public void Remove(ServiceType serviceType) => db.ServiceTypes.Remove(serviceType);
}
