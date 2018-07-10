using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;
using System;

namespace Lykke.Service.PayCallback.AzureRepositories.InvoiceConfirmation
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class InvoiceConfirmationEntity : AzureTableEntity, IInvoiceConfirmation
    {
        public string Id => RowKey;

        public DateTime Sent { get; set; }

        public string UserEmail { get; set; }

        [JsonValueSerializer]
        public CashOut CashOut { get; set; }

        [JsonValueSerializer]
        public InvoiceOperation[] InvoiceList { get; set; }

        public DateTime? SettledInBlockchainDateTime { get; set; }

        public string BlockchainHash { get; set; }

        public InvoiceConfirmationEntity()
        {
        }

        public InvoiceConfirmationEntity(IInvoiceConfirmation invoiceConfirmation)
        {
            Sent = DateTime.UtcNow;
            PartitionKey = GetPartitionKey(Sent);
            RowKey = GetRowKey(Guid.NewGuid().ToString());
            
            UserEmail = invoiceConfirmation.UserEmail;
            CashOut = invoiceConfirmation.CashOut;
            InvoiceList = invoiceConfirmation.InvoiceList;
            SettledInBlockchainDateTime = invoiceConfirmation.SettledInBlockchainDateTime;
            BlockchainHash = invoiceConfirmation.BlockchainHash;
        }

        internal static string GetPartitionKey(DateTime sent)
            => sent.ToString("yyyy-MM-ddTHH:mm");
        internal static string GetRowKey(string id)
            => id;
    }
}
