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

        [Required]
        public DateTime SettledInBlockchainDateTime { get; set; }

        [Required]
        public string BlockchainHash { get; set; }
    }
}
