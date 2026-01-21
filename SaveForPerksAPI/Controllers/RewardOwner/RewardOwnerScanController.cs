using Microsoft.AspNetCore.Mvc;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Services;

namespace SaveForPerksAPI.Controllers.RewardOwner
{
    [Route("api/reward-owner/scans")]
    public class RewardOwnerScanController : BaseApiController
    {
        private readonly IRewardService rewardService;

        public RewardOwnerScanController(
            IRewardService rewardService, 
            ILogger<RewardOwnerScanController> logger)
            : base(logger)
        {
            this.rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        }

        [HttpGet("{rewardId}/events/{scanEventId}", Name = "GetScanEventForReward")]
        public async Task<ActionResult<ScanEventDto>> GetScanEventForReward(Guid rewardId, Guid scanEventId)
        {
            Logger.LogInformation("GetScanEventForReward called with RewardId: {RewardId}, ScanEventId: {ScanEventId}", rewardId, scanEventId);
            
            return await ExecuteAsync(
                () => rewardService.GetScanEventForRewardAsync(rewardId, scanEventId),
                nameof(GetScanEventForReward));
        }

        [HttpGet("{rewardId}/userbalance/{qrCodeValue}", Name = "GetUserBalanceForReward")]
        public async Task<ActionResult<UserBalanceAndInfoResponseDto>> GetUserBalanceForReward(
            Guid rewardId, 
            string qrCodeValue)
        {
            Logger.LogInformation("GetUserBalanceForReward called with RewardId: {RewardId}, QrCodeValue: {QrCodeValue}", rewardId, qrCodeValue);
            
            return await ExecuteAsync(
                () => rewardService.GetUserBalanceForRewardAsync(rewardId, qrCodeValue),
                nameof(GetUserBalanceForReward));
        }

        [HttpPost]
        public async Task<ActionResult<ScanEventResponseDto>> CreatePointsAndClaimRewards(
            ScanEventForCreationDto scanEventForCreationDto)
        {
            Logger.LogInformation("CreatePointsAndClaimRewards called with RewardId: {RewardId}, QrCodeValue: {QrCodeValue}", 
                scanEventForCreationDto.RewardId, scanEventForCreationDto.QrCodeValue);
            
            return await ExecuteCreatedAsync(
                () => rewardService.ProcessScanAndRewardsAsync(scanEventForCreationDto),
                "GetScanEventForReward",
                v => new { rewardId = v.ScanEvent.RewardId, scanEventId = v.ScanEvent.Id },
                nameof(CreatePointsAndClaimRewards));
        }

        [HttpGet("History")]
        public Task<ActionResult<IEnumerable<RewardOwnerDto>>> GetScansHistoryForReward()
        {
            Logger.LogInformation("GetScansHistoryForReward called");
            
            return Task.FromResult<ActionResult<IEnumerable<RewardOwnerDto>>>(Ok(true));
        }
    }
}

