
using System.Threading.Tasks;
using Lykke.Service.PayCallback.Client.Models;

namespace Lykke.Service.PayCallback.Client
{
    public interface IPayCallbackClient
    {
        /// <summary>
        /// Adds callback url for the payment request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task AddPaymentCallback(CreatePaymentCallbackModel request);
    }
}
