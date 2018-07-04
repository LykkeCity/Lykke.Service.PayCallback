using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    public class InvoiceConfirmation : IInvoiceConfirmation
    {
        [Required]
        public string UserEmail { get; set; }

        public CashOut CashOut { get; set; }

        public InvoiceOperation[] InvoiceList { get; set; }

        public DateTime? SettledInBlockchainDateTime { get; set; }

        public string BlockchainHash { get; set; }
    }
}
