using System;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    public interface IInvoiceConfirmation
    {
        string UserEmail { get; }

        InvoiceOperation[] InvoiceList { get; }

        DateTime? SettledInBlockchainDateTime { get; }

        string BlockchainHash { get; }
    }
}
