﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
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

        public async Task CreatePaymentCallback(CreatePaymentCallback request)
        {
            await _paymentCallbackRepository.InsertAsync(request);
        }

        public async Task ProcessPaymentUpdate(PaymentRequestDetailsMessage model)
        {
            var callback = await _paymentCallbackRepository.GetAsync(model.MerchantId, model.Id);

            if (callback == null)
                throw new CallbackNotFoundException(model.Id);

            using (var httpClient = new HttpClient())
            {
                await httpClient.PostAsync(callback.Url, new StringContent(model.ToJson()));
            }
        }
    }
}
