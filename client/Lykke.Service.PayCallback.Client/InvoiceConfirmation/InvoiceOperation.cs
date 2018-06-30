using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    public class InvoiceOperation
    {
        [Required]
        public string InvoiceNumber { get; set; }        
        public decimal? AmountPaid { get; set; }
        public decimal? AmountLeftPaid { get; set; }
        public DisputeOperation Dispute { get; set; }
    }
}
