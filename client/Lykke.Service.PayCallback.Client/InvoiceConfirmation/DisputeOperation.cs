using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    public class DisputeOperation
    {
        [Required]
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime DateTime { get; set; }
    }
}
