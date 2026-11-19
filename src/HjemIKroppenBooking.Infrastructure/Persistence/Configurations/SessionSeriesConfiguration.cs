using HjemIKroppenBooking.Domain.Series;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Configurations;

internal sealed class SessionSeriesConfiguration : IEntityTypeConfiguration<SessionSeries>
{
    public void Configure(EntityTypeBuilder<SessionSeries> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(SessionSeries.TitleMaxLength).IsRequired();

        builder.HasOne(x => x.ServiceType).WithMany().HasForeignKey(x => x.ServiceTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Sessions).WithOne(x => x.Series).HasForeignKey(x => x.SeriesId).OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(x => x.Sessions).HasField("_sessions").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
