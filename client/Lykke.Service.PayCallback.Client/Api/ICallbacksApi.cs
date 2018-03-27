using System.Threading.Tasks;
using Lykke.Service.PayCallback.Client.Models;
using Refit;

namespace Lykke.Service.PayCallback.Client.Api
{
    internal interface ICallbacksApi
    {
        [Post("/api/callback")]
        Task SetPaymentCallback([Body] SetPaymentCallbackModel request);

        [Get("/api/callback/{merchantId}/{paymentRequestId}")]
        Task<GetPaymentCallbackModel> GetPaymentCallback(string merchantId, string paymentRequestId);
    }
}
