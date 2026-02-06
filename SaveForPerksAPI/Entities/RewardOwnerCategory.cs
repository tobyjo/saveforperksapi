namespace SaveForPerksAPI.Entities;

public class RewardOwnerCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    // Navigation property
    public ICollection<RewardOwner> RewardOwners { get; set; } = new List<RewardOwner>();
}
