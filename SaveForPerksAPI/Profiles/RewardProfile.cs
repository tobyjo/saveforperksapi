using AutoMapper;

namespace SaveForPerksAPI.Profiles;

public class RewardProfile : Profile
{
    public RewardProfile()
    {
        CreateMap<Entities.Reward, Models.RewardDto>();
        
        CreateMap<Models.RewardDto, Entities.Reward>()
            .ForMember(dest => dest.Metadata, opt => opt.Ignore())
            .ForMember(dest => dest.Business, opt => opt.Ignore())
            .ForMember(dest => dest.RewardRedemptions, opt => opt.Ignore())
            .ForMember(dest => dest.ScanEvents, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerBalances, opt => opt.Ignore());
    }
}
