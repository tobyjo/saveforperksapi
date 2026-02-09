using Microsoft.AspNetCore.Mvc;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Services;

namespace SaveForPerksAPI.Controllers.Business
{
    [Route("api/business")]
    public class BusinessController : BaseApiController
    {
        private readonly IRewardTransactionService _rewardTransactionService;

        public BusinessController(
            IRewardTransactionService rewardTransactionService,
            ILogger<BusinessScanController> logger)
            : base(logger)
        {
            _rewardTransactionService = rewardTransactionService ?? throw new ArgumentNullException(nameof(rewardTransactionService));
        }

        [HttpPost]
        public async Task<ActionResult<BusinessWithAdminUserResponseDto>> CreateBusinessWithAdminUser(
            BusinessWithAdminUserForCreationDto businessWithAdminUserForCreationDto)
        {
            Logger.LogInformation(
                "CreateBusiness called with BusinessName: {BusinessName}, Email: {Email}, AuthProviderId: {AuthProviderId}",
                businessWithAdminUserForCreationDto.BusinessName, 
                businessWithAdminUserForCreationDto.BusinessUserEmail,
                businessWithAdminUserForCreationDto.BusinessUserAuthProviderId);

            return await ExecuteAsync(
                () => _rewardTransactionService.CreateBusinessAsync(businessWithAdminUserForCreationDto),
                nameof(CreateBusinessWithAdminUser));
        }

    }
}
