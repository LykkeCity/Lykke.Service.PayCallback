using System.Threading.Tasks;

namespace Lykke.Service.PayCallback.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}