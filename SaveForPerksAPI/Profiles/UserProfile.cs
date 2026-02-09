using AutoMapper;

namespace SaveForPerksAPI.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<Entities.Customer, Models.CustomerDto>();
        
        CreateMap<Models.CustomerDto, Entities.Customer>()
            .ForMember(dest => dest.RewardRedemptions, opt => opt.Ignore())
            .ForMember(dest => dest.ScanEvents, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerBalances, opt => opt.Ignore());
    }
}
