using HjemIKroppenBooking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HjemIKroppenBooking.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Email).HasMaxLength(User.EmailMaxLength).IsRequired();
        builder.Property(x => x.FullName).HasMaxLength(User.FullNameMaxLength).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(User.PasswordHashMaxLength).IsRequired();
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName(IndexNames.UserEmail);
        builder.HasIndex(x => x.FullName);
    }
}
