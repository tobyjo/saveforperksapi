using System;
using System.Collections.Generic;

namespace TapForPerksAPI.Entities;

public class LoyaltyProgramme
{
    public Guid Id { get; set; }

    public Guid LoyaltyOwnerId { get; set; }

    public string Name { get; set; } = null!;

    public string RewardType { get; set; } = string.Empty;
    public int? CostPoints { get; set; }

    public bool IsActive { get; set; }

    public string? Metadata { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual LoyaltyOwner LoyaltyOwner { get; set; } = null!;

    public virtual ICollection<RewardRedemption> RewardRedemptions { get; set; } = new List<RewardRedemption>();

    public virtual ICollection<ScanEvent> ScanEvents { get; set; } = new List<ScanEvent>();

    public virtual ICollection<UserBalance> UserBalances { get; set; } = new List<UserBalance>();
}
