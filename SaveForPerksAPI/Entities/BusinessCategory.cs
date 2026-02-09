namespace SaveForPerksAPI.Entities;

public class BusinessCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Business> Businesses { get; set; } = new List<Business>();
}
