using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Lykke.Service.PayCallback.Core.Domain;
using Lykke.Service.PayCallback.Core.Exceptions;
using Lykke.Service.PayCallback.Core.Services;
using Lykke.Service.PayInternal.Contract.PaymentRequest;

namespace Lykke.Service.PayCallback.Services
{
    public class CallbackService : ICallbackService
    {
        private readonly IPaymentCallbackRepository _paymentCallbackRepository;

        public CallbackService(IPaymentCallbackRepository paymentCallbackRepository)
        {
            _paymentCallbackRepository = paymentCallbackRepository ??
                                         throw new ArgumentNullException(nameof(paymentCallbackRepository));
        }

        public async Task SetPaymentRequestCallback(SetPaymentRequestCallbackCommand command)
        {
            await _paymentCallbackRepository.SetAsync(Mapper.Map<PaymentCallback>(command));
        }

        public async Task<IPaymentCallback> GetPaymentRequestCallback(string merchantId, string paymentRequestId)
        {
            return await _paymentCallbackRepository.GetAsync(merchantId, paymentRequestId);
        }

        public async Task ProcessPaymentRequestUpdate(PaymentRequestDetailsMessage model)
        {
            var callback = await _paymentCallbackRepository.GetAsync(model.MerchantId, model.Id);

            if (callback == null)
                throw new CallbackNotFoundException(model.Id);

            using (var httpClient = new HttpClient())
            {
                string content = model.ToStatusApiModel().ToJson();

                await httpClient.PostAsync(callback.Url, new StringContent(content));
            }
        }
    }
}
