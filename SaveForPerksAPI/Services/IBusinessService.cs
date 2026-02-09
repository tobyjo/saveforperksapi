using SaveForPerksAPI.Common;
using SaveForPerksAPI.Models;

namespace SaveForPerksAPI.Services;

public interface IBusinessService
{
    Task<Result<BusinessWithAdminUserResponseDto>> CreateBusinessAsync(BusinessWithAdminUserForCreationDto request);
}
