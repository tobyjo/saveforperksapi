using AutoMapper;

namespace SaveForPerksAPI.Profiles;

public class ScanEventProfile : Profile
{
    public ScanEventProfile()
    {
        // From database to DTO
        CreateMap<Entities.ScanEvent, Models.ScanEventDto>();

        // From DTO to database
        CreateMap<Models.ScanEventForCreationDto, Entities.ScanEvent>()
            .ForMember(dest => dest.BusinessUser, opt => opt.Ignore())
            .ForMember(dest => dest.Reward, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ScannedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore());
    
    }
}
