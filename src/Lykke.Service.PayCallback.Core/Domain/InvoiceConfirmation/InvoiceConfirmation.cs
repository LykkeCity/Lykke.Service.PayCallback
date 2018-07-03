using System;

namespace Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation
{
    public class InvoiceConfirmation : IInvoiceConfirmation
    {
        public string UserEmail { get; set; }

        public InvoiceOperation[] InvoiceList { get; set; }

        public DateTime? SettledInBlockchainDateTime { get; set; }

        public string BlockchainHash { get; set; }
    }
}
