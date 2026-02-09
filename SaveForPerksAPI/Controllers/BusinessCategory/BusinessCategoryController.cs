using Microsoft.AspNetCore.Mvc;
using SaveForPerksAPI.Models;
using SaveForPerksAPI.Services;

namespace SaveForPerksAPI.Controllers.BusinessCategory
{
    [Route("api/business-category")]
    public class BusinessCategoryController : BaseApiController
    {
        private readonly IBusinessCategoryService _businessCategoryService;

        public BusinessCategoryController(
            IBusinessCategoryService businessCategoryService,
            ILogger<BusinessCategoryController> logger)
            : base(logger)
        {
            _businessCategoryService = businessCategoryService ?? throw new ArgumentNullException(nameof(businessCategoryService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessCategoryDto>>> GetAllCategories()
        {
            Logger.LogInformation("GetAllCategories called");

            return await ExecuteAsync(
                () => _businessCategoryService.GetAllCategoriesAsync(),
                nameof(GetAllCategories));
        }
    }
}
