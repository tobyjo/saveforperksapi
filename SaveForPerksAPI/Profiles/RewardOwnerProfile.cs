using AutoMapper;

namespace SaveForPerksAPI.Profiles
{
    public class RewardOwnerProfile : Profile
    {
        public RewardOwnerProfile()
        {
            // From database entity to DTO
            CreateMap<Entities.Business, Models.BusinessDto>();

            // From DTO to database entity
            CreateMap<Models.BusinessDto, Entities.Business>()
                .ForMember(dest => dest.Metadata, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.BusinessUsers, opt => opt.Ignore())
                .ForMember(dest => dest.Rewards, opt => opt.Ignore());
        }
    }
}
