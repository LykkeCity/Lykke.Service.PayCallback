using System.Threading.Tasks;

namespace Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation
{
    public interface IInvoiceConfirmationRepository
    {
        Task AddAsync(IInvoiceConfirmation invoiceConfirmation);
    }
}
