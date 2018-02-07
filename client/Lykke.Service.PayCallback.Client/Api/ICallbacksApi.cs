using System.Threading.Tasks;
using Lykke.Service.PayCallback.Client.Models;
using Refit;

namespace Lykke.Service.PayCallback.Client.Api
{
    public interface ICallbacksApi
    {
        [Post("/api/callback")]
        Task AddPaymentCallback([Body] CreatePaymentCallbackModel request);
    }
}
