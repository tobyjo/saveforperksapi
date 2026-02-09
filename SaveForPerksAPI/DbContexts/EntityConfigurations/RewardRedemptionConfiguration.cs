using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaveForPerksAPI.Entities;

namespace SaveForPerksAPI.DbContexts.EntityConfigurations;

public class RewardRedemptionConfiguration : IEntityTypeConfiguration<RewardRedemption>
{
    public void Configure(EntityTypeBuilder<RewardRedemption> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("reward_redemption");

        builder.HasIndex(e => e.RewardId);
        builder.HasIndex(e => e.CustomerId);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("(newid())")
            .HasColumnName("id");

        builder.Property(e => e.BusinessUserId)
            .HasColumnName("business_user_id");

        builder.Property(e => e.RewardId)
            .HasColumnName("reward_id");

        builder.Property(e => e.RedeemedAt)
            .HasDefaultValueSql("(sysdatetime())")
            .HasColumnName("redeemed_at");

        builder.Property(e => e.CustomerId)
            .HasColumnName("user_id");

        builder.HasOne(d => d.BusinessUser)
            .WithMany(p => p.RewardRedemptions)
            .HasForeignKey(d => d.BusinessUserId);

        builder.HasOne(d => d.Reward)
            .WithMany(p => p.RewardRedemptions)
            .HasForeignKey(d => d.RewardId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(d => d.Customer)
            .WithMany(p => p.RewardRedemptions)
            .HasForeignKey(d => d.CustomerId);

        // No seed data for redemptions initially
    }
}
