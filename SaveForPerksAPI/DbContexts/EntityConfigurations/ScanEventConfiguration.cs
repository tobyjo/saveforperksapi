using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaveForPerksAPI.Entities;

namespace SaveForPerksAPI.DbContexts.EntityConfigurations;

public class ScanEventConfiguration : IEntityTypeConfiguration<ScanEvent>
{
    public void Configure(EntityTypeBuilder<ScanEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("scan_event");

        builder.HasIndex(e => e.RewardId);
        builder.HasIndex(e => e.CustomerId);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("(newid())")
            .HasColumnName("id");

        builder.Property(e => e.BusinessUserId)
            .HasColumnName("business_user_id");

        builder.Property(e => e.RewardId)
            .HasColumnName("reward_id");

        builder.Property(e => e.PointsChange)
            .HasDefaultValue(1)
            .HasColumnName("points_change");

        builder.Property(e => e.ScannedAt)
            .HasDefaultValueSql("(sysdatetime())")
            .HasColumnName("scanned_at");

        builder.Property(e => e.CustomerId)
            .HasColumnName("customer_id");

        builder.Property(e => e.QrCodeValue)
         .HasColumnName("qr_code_value");

        builder.HasOne(d => d.BusinessUser)
            .WithMany(p => p.ScanEvents)
            .HasForeignKey(d => d.BusinessUserId); ;

        builder.HasOne(d => d.Reward)
            .WithMany(p => p.ScanEvents)
            .HasForeignKey(d => d.RewardId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.Customer)
            .WithMany(p => p.ScanEvents)
            .HasForeignKey(d => d.CustomerId);

        // No seed data for scan events initially
    }
}
