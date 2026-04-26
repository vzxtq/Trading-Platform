using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingEngine.Domain.Entities;
using TradingEngine.Domain.ValueObjects;

namespace TradingEngine.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<OrderDomain>
{
    public void Configure(EntityTypeBuilder<OrderDomain> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id)
            .HasName("PK_Orders");

        builder.HasIndex(o => new { o.UserId, o.CreatedAt })
            .HasDatabaseName("IX_Orders_User_CreatedAt");

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.Side)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.Symbol)
               .HasConversion(
                   v => v.Value,
                   v => new Symbol(v))
               .HasColumnName("Symbol")
               .HasMaxLength(10)
               .IsRequired();

        builder.Property(o => o.Price)
               .HasConversion(v => v.Value, v => new Price(v))
               .HasColumnName("Price")
               .HasPrecision(18, 8)
               .IsRequired();

        builder.Property(o => o.Quantity)
               .HasConversion(v => v.Value, v => new Quantity(v))
               .HasColumnName("Quantity")
               .HasPrecision(18, 8)
               .IsRequired();

        builder.Property(o => o.RemainingQuantity)
               .HasConversion(v => v.Value, v => new Quantity(v))
               .HasColumnName("RemainingQuantity")
               .HasPrecision(18, 8)
               .IsRequired();

        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt);
    }
}
