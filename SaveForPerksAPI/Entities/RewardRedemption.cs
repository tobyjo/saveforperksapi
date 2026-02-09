using System;
using System.Collections.Generic;

namespace SaveForPerksAPI.Entities;

public class RewardRedemption
{
    public Guid Id { get; set; }


    public Guid CustomerId { get; set; }

    public Guid RewardId { get; set; }

    public Guid? BusinessUserId { get; set; }

    public DateTime RedeemedAt { get; set; }

    public virtual BusinessUser? BusinessUser { get; set; }

    public virtual Reward Reward { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
