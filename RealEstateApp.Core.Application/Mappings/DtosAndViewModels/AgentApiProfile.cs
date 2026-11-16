using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Agent;
using RealEstateApp.Core.Application.Dtos.User;

namespace RealEstateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class AgentApiProfile : Profile
    {
        public AgentApiProfile()
        {
            /*CreateMap<UserSummaryDto, AgentApiDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src))
                .ReverseMap();*/
        }
    }
}
