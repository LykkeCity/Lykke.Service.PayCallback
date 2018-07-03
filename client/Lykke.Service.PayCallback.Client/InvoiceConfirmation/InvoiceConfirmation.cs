using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    public class InvoiceConfirmation : IInvoiceConfirmation
    {
        [Required]
        public string UserEmail { get; set; }

        [Required]
        public InvoiceOperation[] InvoiceList { get; set; }

        public DateTime? SettledInBlockchainDateTime { get; set; }

        public string BlockchainHash { get; set; }
    }
}
