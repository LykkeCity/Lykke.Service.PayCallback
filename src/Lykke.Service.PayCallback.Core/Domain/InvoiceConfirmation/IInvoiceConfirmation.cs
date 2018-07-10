using System;

namespace Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation
{
    public interface IInvoiceConfirmation
    {
        string UserEmail { get; }

        CashOut CashOut { get; }

        InvoiceOperation[] InvoiceList { get; }

        DateTime? SettledInBlockchainDateTime { get; }

        string BlockchainHash { get; }
    }
}
