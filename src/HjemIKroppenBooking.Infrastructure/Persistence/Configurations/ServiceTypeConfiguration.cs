using HjemIKroppenBooking.Domain.ServiceTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Configurations;

internal sealed class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(ServiceType.NameMaxLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(ServiceType.DescriptionMaxLength);

        builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CategoryId, x.Name }).IsUnique().HasDatabaseName(IndexNames.ServiceTypeName);
    }
}
