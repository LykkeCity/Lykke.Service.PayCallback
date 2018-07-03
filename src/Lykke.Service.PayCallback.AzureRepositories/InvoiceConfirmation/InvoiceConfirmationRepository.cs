using AzureStorage;
using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;
using System.Threading.Tasks;

namespace Lykke.Service.PayCallback.AzureRepositories.InvoiceConfirmation
{
    public class InvoiceConfirmationRepository : IInvoiceConfirmationRepository
    {
        private readonly INoSQLTableStorage<InvoiceConfirmationEntity> _storage;

        public InvoiceConfirmationRepository(INoSQLTableStorage<InvoiceConfirmationEntity> storage)
        {
            _storage = storage;
        }

        public async Task AddAsync(IInvoiceConfirmation invoiceConfirmation)
        {
            await _storage.InsertOrReplaceAsync(new InvoiceConfirmationEntity(invoiceConfirmation));
        }
    }
}
