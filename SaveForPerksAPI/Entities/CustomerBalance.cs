using System;
using System.Collections.Generic;

namespace SaveForPerksAPI.Entities;

public class CustomerBalance
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid RewardId { get; set; }

    public int Balance { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual Reward Reward { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
