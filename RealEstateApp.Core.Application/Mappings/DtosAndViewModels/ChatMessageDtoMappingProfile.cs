using AutoMapper;
using RealEstateApp.Core.Application.Dtos.Chat;
using RealEstateApp.Core.Application.ViewModels.Chat;

namespace RealEstateApp.Core.Application.Mappings.DtosAndViewModels
{
    public class ChatMessageDtoMappingProfile : Profile
    {
        public ChatMessageDtoMappingProfile()
        {
            CreateMap<ChatMessageDto, ChatMessageViewModel>();
            CreateMap<ConversationSummaryDto, ConversationSummaryViewModel>();
        }
    }
}