using System.Threading.Tasks;

namespace Lykke.Service.PayCallback.Core.Domain
{
    public interface ICallbackService
    {
        Task CreatePaymentCallback(CreatePaymentCallback request);
    }
}
