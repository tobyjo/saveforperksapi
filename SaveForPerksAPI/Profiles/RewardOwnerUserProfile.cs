using AutoMapper;

namespace SaveForPerksAPI.Profiles;

public class RewardOwnerUserProfile : Profile
{
    public RewardOwnerUserProfile()
    {
        CreateMap<Entities.BusinessUser, Models.BusinessUserDto>();
        
        CreateMap<Models.BusinessUserDto, Entities.BusinessUser>()
            .ForMember(dest => dest.Business, opt => opt.Ignore())
            .ForMember(dest => dest.RewardRedemptions, opt => opt.Ignore())
            .ForMember(dest => dest.ScanEvents, opt => opt.Ignore());
    }
}
