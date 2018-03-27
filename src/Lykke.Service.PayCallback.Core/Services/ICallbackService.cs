using System.Threading.Tasks;
using Lykke.Service.PayCallback.Core.Domain;
using Lykke.Service.PayInternal.Contract.PaymentRequest;

namespace Lykke.Service.PayCallback.Core.Services
{
    public interface ICallbackService
    {
        Task SetPaymentRequestCallback(SetPaymentRequestCallbackCommand command);

        Task<IPaymentCallback> GetPaymentRequestCallback(string merchantId, string paymentRequestId);

        Task ProcessPaymentRequestUpdate(PaymentRequestDetailsMessage model);
    }
}
