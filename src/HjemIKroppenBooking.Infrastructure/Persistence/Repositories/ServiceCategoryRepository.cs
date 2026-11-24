using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.ServiceCategories;
using Microsoft.EntityFrameworkCore;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Repositories;

internal sealed class ServiceCategoryRepository(AppDbContext db) : IServiceCategoryRepository
{
    public Task<ServiceCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.ServiceCategories.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken cancellationToken) =>
        db.ServiceCategories.AnyAsync(c => c.Name == name && (excludeId == null || c.Id != excludeId), cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken cancellationToken) =>
        db.ServiceCategories.AnyAsync(c => c.Slug == slug && (excludeId == null || c.Id != excludeId), cancellationToken);

    public Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken) =>
        db.ServiceTypes.AnyAsync(s => s.CategoryId == id, cancellationToken);

    public void Add(ServiceCategory category) => db.ServiceCategories.Add(category);

    public void Remove(ServiceCategory category) => db.ServiceCategories.Remove(category);
}
