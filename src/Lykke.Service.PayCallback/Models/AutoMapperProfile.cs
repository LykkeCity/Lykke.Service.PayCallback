using AutoMapper;
using Lykke.Service.PayCallback.Core.Domain;

namespace Lykke.Service.PayCallback.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SetPaymentCallbackRequest, SetPaymentRequestCallbackCommand>(MemberList.Source);
        }
    }
}
