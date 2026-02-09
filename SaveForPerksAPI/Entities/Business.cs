using System;
using System.Collections.Generic;


namespace SaveForPerksAPI.Entities;

public class Business
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;        
    public string? Address { get; set; }

    public string? Metadata { get; set; }

    public Guid? CategoryId { get; set; }  // Foreign key to BusinessCategory

    public DateTime CreatedAt { get; set; }

    public virtual BusinessCategory? Category { get; set; }  // Navigation property

    public virtual ICollection<BusinessUser> BusinessUsers { get; set; } = new List<BusinessUser>();

    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();
}
