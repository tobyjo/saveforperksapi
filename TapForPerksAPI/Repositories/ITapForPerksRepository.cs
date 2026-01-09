using TapForPerksAPI.Entities;

namespace TapForPerksAPI.Repositories
{
    public interface ITapForPerksRepository
    {
        Task<IEnumerable<LoyaltyOwner>> GetLoyaltyOwnersAsync();

    }
}
