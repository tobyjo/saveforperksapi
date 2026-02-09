using Microsoft.AspNetCore.Mvc;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Services;

namespace SaveForPerksAPI.Controllers.Business
{
    [Route("api/business/scans")]
    public class BusinessScanController : BaseApiController
    {
        private readonly IRewardTransactionService _rewardTransactionService;

        public BusinessScanController(
            IRewardTransactionService rewardTransactionService, 
            ILogger<BusinessScanController> logger)
            : base(logger)
        {
            _rewardTransactionService = rewardTransactionService ?? throw new ArgumentNullException(nameof(rewardTransactionService));
        }

        [HttpGet("{rewardId}/events/{scanEventId}", Name = "GetScanEventForReward")]
        public async Task<ActionResult<ScanEventDto>> GetScanEventForReward(Guid rewardId, Guid scanEventId)
        {
            Logger.LogInformation("GetScanEventForReward called with RewardId: {RewardId}, ScanEventId: {ScanEventId}", rewardId, scanEventId);
            
            return await ExecuteAsync(
                () => _rewardTransactionService.GetScanEventForRewardAsync(rewardId, scanEventId),
                nameof(GetScanEventForReward));
        }

        [HttpGet("{rewardId}/customerbalance/{qrCodeValue}", Name = "GetCustomerBalanceForReward")]
        public async Task<ActionResult<CustomerBalanceAndInfoResponseDto>> GetCustomerBalanceForReward(
            Guid rewardId, 
            string qrCodeValue)
        {
            Logger.LogInformation("GetCustomerBalanceForReward called with RewardId: {RewardId}, QrCodeValue: {QrCodeValue}", rewardId, qrCodeValue);
            
            return await ExecuteAsync(
                () => _rewardTransactionService.GetCustomerBalanceForRewardAsync(rewardId, qrCodeValue),
                nameof(GetCustomerBalanceForReward));
        }

        [HttpPost]
        public async Task<ActionResult<ScanEventResponseDto>> CreatePointsAndClaimRewards(
            ScanEventForCreationDto scanEventForCreationDto)
        {
            Logger.LogInformation("CreatePointsAndClaimRewards called with RewardId: {RewardId}, QrCodeValue: {QrCodeValue}", 
                scanEventForCreationDto.RewardId, scanEventForCreationDto.QrCodeValue);
            
            return await ExecuteCreatedAsync(
                () => _rewardTransactionService.ProcessScanAndRewardsAsync(scanEventForCreationDto),
                "GetScanEventForReward",
                v => new { rewardId = v.ScanEvent.RewardId, scanEventId = v.ScanEvent.Id },
                nameof(CreatePointsAndClaimRewards));
        }

        [HttpGet("History")]
        public Task<ActionResult<IEnumerable<BusinessDto>>> GetScansHistoryForReward()
        {
            Logger.LogInformation("GetScansHistoryForReward called");
            
            return Task.FromResult<ActionResult<IEnumerable<BusinessDto>>>(Ok(true));
        }
    }
}

