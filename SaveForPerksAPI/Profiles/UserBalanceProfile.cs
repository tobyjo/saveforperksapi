using AutoMapper;

namespace SaveForPerksAPI.Profiles;

public class UserBalanceProfile : Profile
{
    public UserBalanceProfile()
    {
        CreateMap<Entities.CustomerBalance, Models.CustomerBalanceDto>();
        
        CreateMap<Models.CustomerBalanceDto, Entities.CustomerBalance>()
            .ForMember(dest => dest.Reward, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore());
    }
}
