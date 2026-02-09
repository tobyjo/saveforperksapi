using SaveForPerksAPI.Common;
using SaveForPerksAPI.Models;

namespace SaveForPerksAPI.Services;

public interface IBusinessUserService
{
    Task<Result<IEnumerable<BusinessDto>>> GetBusinessesByAuthProviderIdAsync(string authProviderId);

    Task<Result<BusinessUserDto>> GetBusinessUserByAuthProviderIdAsync(string authProviderId);

    Task<Result<IEnumerable<BusinessUserProfileResponseDto>>> GetBusinessUserProfilesByAuthProviderIdAsync(string authProviderId);
}
