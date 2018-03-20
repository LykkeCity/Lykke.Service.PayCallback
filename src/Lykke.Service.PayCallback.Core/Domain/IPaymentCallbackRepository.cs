using System.Threading.Tasks;

namespace Lykke.Service.PayCallback.Core.Domain
{
    public interface IPaymentCallbackRepository
    {
        Task<IPaymentCallback> SetAsync(IPaymentCallback paymentCallback);

        Task<IPaymentCallback> GetAsync(string merchantId, string paymentRequestId);
    }
}
