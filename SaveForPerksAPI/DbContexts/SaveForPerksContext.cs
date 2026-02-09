using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SaveForPerksAPI.Entities;

namespace SaveForPerksAPI.DbContexts;

public class SaveForPerksContext : DbContext
{
    public SaveForPerksContext(DbContextOptions<SaveForPerksContext> options)
        : base(options)
    {
    }

    public DbSet<Business> Businesses { get; set; }
    public DbSet<BusinessCategory> BusinessCategories { get; set; }
    public DbSet<BusinessUser> BusinessUsers { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<RewardRedemption> RewardRedemptions { get; set; }
    public DbSet<ScanEvent> ScanEvents { get; set; }
    public DbSet<Customer> Customer { get; set; }
    public DbSet<CustomerBalance> CustomerBalances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SaveForPerksContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
