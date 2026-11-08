using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Chat;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDtos
{
    public class ChatMessageMappingProfile : Profile
    {
        public ChatMessageMappingProfile()
        {
            CreateMap<ChatMessage, ChatMessageDto>();
            
            CreateMap<ChatMessageDto, ChatMessage>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.SentAt, o => o.Ignore())
                .ForMember(d => d.Property, o => o.Ignore());
        }
    }
}