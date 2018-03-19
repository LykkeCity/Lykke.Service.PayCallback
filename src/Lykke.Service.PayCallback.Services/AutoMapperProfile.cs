using AutoMapper;
using Lykke.Service.PayCallback.Core.Domain;

namespace Lykke.Service.PayCallback.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SetPaymentRequestCallbackCommand, PaymentCallback>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.CallbackUrl));
        }
    }
}
