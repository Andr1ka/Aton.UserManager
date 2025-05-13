using AutoMapper;
using Domain;
using Models.Responses;

namespace Models.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserSummaryResponse>()
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.Admin));
            CreateMap<User, UserDetailResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.RevokedOn == null));
        }
    }
} 