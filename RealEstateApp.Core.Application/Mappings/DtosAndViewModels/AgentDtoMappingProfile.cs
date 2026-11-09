using AutoMapper;
using RealEstateApp.Core.Application.Dtos.User;
using RealEstateApp.Core.Application.ViewModels.Agent;

namespace RealEstateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class AgentDtoMappingProfile : Profile
    {
        public AgentDtoMappingProfile()
        {
            CreateMap<AgentBasicDto, AgentViewModel>();
        }
    }
}