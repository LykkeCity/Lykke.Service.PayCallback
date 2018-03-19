using System.Threading.Tasks;
using Lykke.Service.PayCallback.Core.Domain;
using Lykke.Service.PayInternal.Contract.PaymentRequest;

namespace Lykke.Service.PayCallback.Core.Services
{
    public interface ICallbackService
    {
        Task SetPaymentRequestCallback(SetPaymentRequestCallbackCommand command);

        Task ProcessPaymentRequestUpdate(PaymentRequestDetailsMessage model);
    }
}
