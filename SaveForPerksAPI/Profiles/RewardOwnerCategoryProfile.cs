using AutoMapper;
using SaveForPerksAPI.Entities;
using SaveForPerksAPI.Models;

namespace SaveForPerksAPI.Profiles;

public class RewardOwnerCategoryProfile : Profile
{
    public RewardOwnerCategoryProfile()
    {
        CreateMap<RewardOwnerCategory, RewardOwnerCategoryDto>();
    }
}
