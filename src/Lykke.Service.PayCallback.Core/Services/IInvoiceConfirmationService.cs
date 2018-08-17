using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;
using System.Threading.Tasks;

namespace Lykke.Service.PayCallback.Core.Services
{
    public interface IInvoiceConfirmationService
    {
        Task ProcessAsync(InvoiceConfirmation invoiceConfirmation);
    }
}
