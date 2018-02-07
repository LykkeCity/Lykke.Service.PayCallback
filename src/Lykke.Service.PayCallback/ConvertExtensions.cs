using Lykke.Service.PayCallback.Core.Domain;
using Lykke.Service.PayCallback.Models;

namespace Lykke.Service.PayCallback
{
    public static class ConvertExtensions
    {
        public static CreatePaymentCallback ToDomain(this CreatePaymentCallbackRequest src)
        {
            return new CreatePaymentCallback
            {
                MerchantId = src.MerchantId,
                PaymentRequestId = src.PaymentRequestId,
                Url = src.CallbackUrl
            };
        }
    }
}
