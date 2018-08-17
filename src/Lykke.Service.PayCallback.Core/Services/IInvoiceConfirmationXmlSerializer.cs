using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;

namespace Lykke.Service.PayCallback.Core.Services
{
    public interface IInvoiceConfirmationXmlSerializer
    {
        string Serialize(IInvoiceConfirmation invoiceConfirmation);
    }
}
