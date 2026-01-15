using TapForPerksAPI.Common;
using TapForPerksAPI.Models;

namespace TapForPerksAPI.Services;

public interface IRewardService
{
    Task<Result<ScanEventResponseDto>> ProcessScanAndRewardsAsync(
        ScanEventForCreationDto request);
    
    Task<Result<UserBalanceAndInfoResponseDto>> GetUserBalanceForRewardAsync(
        Guid rewardId, 
        string qrCodeValue);
    
    Task<Result<ScanEventDto>> GetScanEventForRewardAsync(
        Guid rewardId, 
        Guid scanEventId);
}
