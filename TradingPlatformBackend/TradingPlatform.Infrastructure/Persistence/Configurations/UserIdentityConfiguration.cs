using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingEngine.Domain.Entities;

namespace TradingEngine.Infrastructure.Persistence.Configurations;

public sealed class UserIdentityConfiguration : IEntityTypeConfiguration<UserIdentityDomain>
{
    public void Configure(EntityTypeBuilder<UserIdentityDomain> builder)
    {
        builder.ToTable("UserIdentities");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId)
               .IsUnique();

        builder.Property(x => x.PasswordHash)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("SYSUTCDATETIME()");
    }
}
