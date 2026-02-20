using System;
using System.Collections.Generic;

namespace SaveForPerksAPI.Entities;

public class Reward
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public int CostPoints { get; set; }
    public RewardType RewardType { get; set; }  // Changed from string to enum
    public int? ExpireDays { get; set; }  // Number of days before points/rewards expire
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Business Business { get; set; } = null!;
    public virtual ICollection<RewardRedemption> RewardRedemptions { get; set; } = new List<RewardRedemption>();
    public virtual ICollection<ScanEvent> ScanEvents { get; set; } = new List<ScanEvent>();
    public virtual ICollection<CustomerBalance> CustomerBalances { get; set; } = new List<CustomerBalance>();
}
