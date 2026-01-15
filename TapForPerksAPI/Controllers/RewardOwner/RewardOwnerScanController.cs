using Microsoft.AspNetCore.Mvc;
using TapForPerksAPI.Models;
using TapForPerksAPI.Services;

namespace TapForPerksAPI.Controllers.RewardOwner
{
    [ApiController]
    [Route("api/reward-owner/scans")]
    public class RewardOwnerScanController : ControllerBase
    {
        private readonly IRewardService rewardService;

        public RewardOwnerScanController(IRewardService rewardService)
        {
            this.rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        }

        [HttpGet("{rewardId}/events/{scanEventId}", Name = "GetScanEventForReward")]
        public async Task<ActionResult<ScanEventDto>> GetScanEventForReward(Guid rewardId, Guid scanEventId)
        {
            var result = await rewardService.GetScanEventForRewardAsync(rewardId, scanEventId);
            
            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{rewardId}/userbalance/{qrCodeValue}", Name = "GetUserBalanceForReward")]
        public async Task<ActionResult<UserBalanceAndInfoResponseDto>> GetUserBalanceForReward(
            Guid rewardId, 
            string qrCodeValue)
        {
            var result = await rewardService.GetUserBalanceForRewardAsync(rewardId, qrCodeValue);
            
            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<ScanEventResponseDto>> CreatePointsAndClaimRewards(
            ScanEventForCreationDto scanEventForCreationDto)
        {
            var result = await rewardService.ProcessScanAndRewardsAsync(scanEventForCreationDto);
            
            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtRoute("GetScanEventForReward",
                new { rewardId = result.Value!.ScanEvent.RewardId, scanEventId = result.Value.ScanEvent.Id },
                result.Value);
        }

        [HttpGet("History")]
        public async Task<ActionResult<IEnumerable<RewardOwnerDto>>> GetScansHistoryForReward()
        {
       
            return Ok(true);
        }
    }
}
