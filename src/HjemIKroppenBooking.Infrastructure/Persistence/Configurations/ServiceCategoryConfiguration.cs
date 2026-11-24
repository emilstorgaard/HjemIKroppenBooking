using HjemIKroppenBooking.Domain.ServiceCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Configurations;

internal sealed class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(ServiceCategory.NameMaxLength).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(ServiceCategory.SlugMaxLength).IsRequired();

        builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName(IndexNames.ServiceCategoryName);
        builder.HasIndex(x => x.Slug).IsUnique().HasDatabaseName(IndexNames.ServiceCategorySlug);
    }
}
