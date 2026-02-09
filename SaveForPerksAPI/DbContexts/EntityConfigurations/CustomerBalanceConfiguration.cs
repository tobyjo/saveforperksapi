using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaveForPerksAPI.Entities;

namespace SaveForPerksAPI.DbContexts.EntityConfigurations;

public class CustomerBalanceConfiguration : IEntityTypeConfiguration<CustomerBalance>
{
    public void Configure(EntityTypeBuilder<CustomerBalance> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("customer_balance");

        builder.HasIndex(e => e.RewardId);
        builder.HasIndex(e => new { e.CustomerId, e.RewardId }).IsUnique();

        builder.Property(e => e.Id)
            .HasDefaultValueSql("(newid())")
            .HasColumnName("id");

        builder.Property(e => e.Balance)
            .HasColumnName("balance");

        builder.Property(e => e.LastUpdated)
            .HasDefaultValueSql("(sysdatetime())")
            .HasColumnName("last_updated");

        builder.Property(e => e.RewardId)
            .HasColumnName("reward_id");

        builder.Property(e => e.CustomerId)
            .HasColumnName("customer_id");

        builder.HasOne(d => d.Reward)
            .WithMany(p => p.CustomerBalances)
            .HasForeignKey(d => d.RewardId);

        builder.HasOne(d => d.Customer)
            .WithMany(p => p.CustomerBalances)
            .HasForeignKey(d => d.CustomerId);


      
    }
}
