using SaveForPerksAPI.Common;
using SaveForPerksAPI.Models;

namespace SaveForPerksAPI.Services;

public interface IRewardManagementService
{
    Task<Result<RewardDto>> CreateRewardAsync(RewardForCreationDto request, Guid businessUserId);

    Task<Result<IEnumerable<RewardDto>>> GetRewardsByBusinessIdAsync(Guid businessId, Guid businessUserId);
}
