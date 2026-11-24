using System.Globalization;
using HjemIKroppenBooking.Application.Abstractions.Persistence;
using HjemIKroppenBooking.Domain.Common;
using HjemIKroppenBooking.Domain.ServiceCategories;

namespace HjemIKroppenBooking.Application.Features.ServiceCategories.Common;

internal static class ServiceCategoryUniqueness
{
    /// <summary>Gives a clear error before saving. The unique index still guards against races.</summary>
    public static async Task EnsureNameIsFreeAsync(this IServiceCategoryRepository categories, string name, Guid? excludeId, CancellationToken cancellationToken)
    {
        if (await categories.NameExistsAsync(name.Trim(), excludeId, cancellationToken))
        {
            throw DomainException.Conflict(ErrorCodes.ServiceCategoryNameAlreadyExists);
        }
    }

    /// <summary>The slug for the name, with "-2", "-3" … added when another category already uses it.</summary>
    public static async Task<string> UniqueSlugAsync(this IServiceCategoryRepository categories, string name, Guid? excludeId, CancellationToken cancellationToken)
    {
        var baseSlug = ServiceCategory.SlugFrom(name);
        var slug = baseSlug;
        for (var suffix = 2; await categories.SlugExistsAsync(slug, excludeId, cancellationToken); suffix++)
        {
            slug = string.Create(CultureInfo.InvariantCulture, $"{baseSlug}-{suffix}");
        }

        return slug;
    }
}
