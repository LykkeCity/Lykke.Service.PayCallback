using AutoMapper;
using Lykke.Service.PayCallback.Core.Domain;

namespace Lykke.Service.PayCallback.AzureRepositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PaymentCallbackEntity, PaymentCallback>(MemberList.Destination);
        }
    }
}
