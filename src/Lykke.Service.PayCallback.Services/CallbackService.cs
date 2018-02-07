using System;
using System.Threading.Tasks;
using Lykke.Service.PayCallback.Core.Domain;

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
    }
}
