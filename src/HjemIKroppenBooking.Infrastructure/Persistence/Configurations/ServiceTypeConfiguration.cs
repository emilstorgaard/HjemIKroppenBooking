using HjemIKroppenBooking.Domain.ServiceTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Configurations;

internal sealed class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> builder)
    {
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(ServiceType.NameMaxLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(ServiceType.DescriptionMaxLength);
        builder.Ignore(x => x.IsGroup);

        builder.HasIndex(x => new { x.Category, x.Name }).IsUnique().HasDatabaseName(IndexNames.ServiceTypeName);
    }
}
