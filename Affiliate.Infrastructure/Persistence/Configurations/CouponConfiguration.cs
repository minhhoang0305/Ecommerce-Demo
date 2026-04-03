using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupon");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).HasMaxLength(50).IsRequired();
        builder.Property(c => c.DiscountType).HasConversion<int>().IsRequired();
        builder.Property(c => c.Value).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(c => c.MinOrderValue).HasColumnType("decimal(18,2)").HasDefaultValue(0);
        builder.Property(c => c.StartDate).IsRequired();
        builder.Property(c => c.EndDate).IsRequired();
        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.Property(c => c.UsageLimit).HasDefaultValue(0);
        builder.Property(c => c.TimesUsed).HasDefaultValue(0);
        builder.HasIndex(c => c.Code).IsUnique();
    }
}
