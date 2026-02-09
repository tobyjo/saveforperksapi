using SaveForPerksAPI.Common;
using SaveForPerksAPI.Models;

namespace SaveForPerksAPI.Services;

public interface IRewardTransactionService
{
    Task<Result<BusinessWithAdminUserResponseDto>> CreateBusinessAsync(
        BusinessWithAdminUserForCreationDto businessWithAdminUserForCreationDto);

    Task<Result<ScanEventResponseDto>> ProcessScanAndRewardsAsync(
        ScanEventForCreationDto request);
    
    Task<Result<CustomerBalanceAndInfoResponseDto>> GetCustomerBalanceForRewardAsync(
        Guid rewardId, 
        string qrCodeValue);
    
    Task<Result<ScanEventDto>> GetScanEventForRewardAsync(
        Guid rewardId, 
        Guid scanEventId);
}
