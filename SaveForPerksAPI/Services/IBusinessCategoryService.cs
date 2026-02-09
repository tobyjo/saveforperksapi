using SaveForPerksAPI.Common;
using SaveForPerksAPI.Models;

namespace SaveForPerksAPI.Services;

public interface IBusinessCategoryService
{
    Task<Result<IEnumerable<BusinessCategoryDto>>> GetAllCategoriesAsync();
}
