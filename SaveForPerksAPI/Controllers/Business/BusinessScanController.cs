using Microsoft.AspNetCore.Mvc;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Services;

namespace SaveForPerksAPI.Controllers.Business
{
    [Route("api/business/{businessId}/scans")]
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
        public async Task<ActionResult<ScanEventDto>> GetScanEventForReward(
            Guid businessId,
            Guid rewardId, 
            Guid scanEventId)
        {
            Logger.LogInformation(
                "GetScanEventForReward called with BusinessId: {BusinessId}, RewardId: {RewardId}, ScanEventId: {ScanEventId}", 
                businessId, rewardId, scanEventId);

            return await ExecuteAsync(
                () => _rewardTransactionService.GetScanEventForRewardAsync(businessId, rewardId, scanEventId),
                nameof(GetScanEventForReward));
        }

        [HttpGet("{rewardId}/customerbalance/{qrCodeValue}", Name = "GetCustomerBalanceForReward")]
        public async Task<ActionResult<CustomerBalanceAndInfoResponseDto>> GetCustomerBalanceForReward(
            Guid businessId,
            Guid rewardId, 
            string qrCodeValue)
        {
            Logger.LogInformation(
                "GetCustomerBalanceForReward called with BusinessId: {BusinessId}, RewardId: {RewardId}, QrCodeValue: {QrCodeValue}", 
                businessId, rewardId, qrCodeValue);

            return await ExecuteAsync(
                () => _rewardTransactionService.GetCustomerBalanceForRewardAsync(businessId, rewardId, qrCodeValue),
                nameof(GetCustomerBalanceForReward));
        }

        [HttpPost]
        public async Task<ActionResult<ScanEventResponseDto>> CreatePointsAndClaimRewards(
            Guid businessId,
            ScanEventForCreationDto scanEventForCreationDto,
            [FromHeader(Name = "X-BusinessUser-Id")] Guid businessUserId)
        {
            Logger.LogInformation(
                "CreatePointsAndClaimRewards called with BusinessId: {BusinessId}, BusinessUserId: {BusinessUserId}, RewardId: {RewardId}, QrCodeValue: {QrCodeValue}", 
                businessId, businessUserId, scanEventForCreationDto.RewardId, scanEventForCreationDto.QrCodeValue);

            return await ExecuteCreatedAsync(
                () => _rewardTransactionService.ProcessScanAndRewardsAsync(businessId, businessUserId, scanEventForCreationDto),
                "GetScanEventForReward",
                v => new { businessId = businessId, rewardId = v.ScanEvent.RewardId, scanEventId = v.ScanEvent.Id },
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

