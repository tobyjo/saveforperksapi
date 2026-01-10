using Microsoft.AspNetCore.Identity;
using TapForPerksAPI.Entities;

namespace TapForPerksAPI.Repositories
{
    public interface ITapForPerksRepository
    {
        Task<bool> SaveChangesAsync();

        Task<IEnumerable<LoyaltyOwner>> GetLoyaltyOwnersAsync();

        Task<ScanEvent?> GetScanEventAsync(Guid loyaltyProgrammeId, Guid scanEventId);

        Task AddScanEvent(ScanEvent scanEvent);

        Task<User?> GetUserByQrCodeValueAsync(string qrCodeValue);

    }
}
