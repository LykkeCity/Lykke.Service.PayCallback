using System;

namespace Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation
{
    public class DisputeOperation
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime DateTime { get; set; }
    }
}
