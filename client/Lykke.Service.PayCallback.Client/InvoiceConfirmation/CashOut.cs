using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    public class CashOut
    {
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }
    }
}
