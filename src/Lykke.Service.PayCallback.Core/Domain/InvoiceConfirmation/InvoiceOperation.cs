namespace Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation
{
    public class InvoiceOperation
    {
        public string InvoiceNumber { get; set; }        
        public decimal? AmountPaid { get; set; }
        public decimal? AmountLeftPaid { get; set; }
        public DisputeOperation Dispute { get; set; }
    }
}
