using Microsoft.AspNetCore.Mvc;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Services;

namespace SaveForPerksAPI.Controllers.RewardOwner
{
    public class RewardOwnerController : BaseApiController
    {
        private readonly IRewardService rewardService;

        public RewardOwnerController(
            IRewardService rewardService,
            ILogger<RewardOwnerScanController> logger)
            : base(logger)
        {
            this.rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
        }

        [HttpPost]
        public async Task<ActionResult<RewardOwnerWithAdminUserForCreationDto>> CreateRewardOwnerWithAdminUser(
            RewardOwnerWithAdminUserForCreationDto rewardOwnerWithAdminUserForCreationDto)
        {
            Logger.LogInformation("CreateRewardOwner called with RewardOwnerName: {RewardOwnerName}, RewardOwnerUserAuthProviderId: {RewardOwnerUserAuthProviderId}",
                rewardOwnerWithAdminUserForCreationDto.RewardOwnerName, rewardOwnerWithAdminUserForCreationDto.RewardOwnerUserAuthProviderId);
     
       

        }

    }
}
