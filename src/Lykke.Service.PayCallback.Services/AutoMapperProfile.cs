using AutoMapper;
using Common;
using Lykke.Service.PayCallback.Core.Domain;
using Lykke.Service.PayInternal.Contract.PaymentRequest;

namespace Lykke.Service.PayCallback.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SetPaymentRequestCallbackCommand, PaymentCallback>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.CallbackUrl));

            CreateMap<PaymentRequestTransaction, PaymentTransactionModel>(MemberList.Destination)
                .ForMember(dest => dest.Timestamp, opt => opt.ResolveUsing(src => src.FirstSeen.ToIsoDateTime()))
                .ForMember(dest => dest.NumberOfConfirmations, opt => opt.MapFrom(src => src.Confirmations))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Id));

            CreateMap<PaymentRequestRefundTransaction, RefundTransactionModel>(MemberList.Destination)
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToIsoDateTime()))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Hash));

            CreateMap<PaymentRequestRefund, RefundRequestModel>(MemberList.Destination)
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Timestamp.ToIsoDateTime()))
                .ForMember(dest => dest.ExpirationDt, opt => opt.MapFrom(src => src.DueDate.ToIsoDateTime()))
                .ForMember(dest => dest.Error, opt => opt.Ignore());
        }
    }
}
