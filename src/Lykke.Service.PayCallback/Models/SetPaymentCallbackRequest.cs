using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayCallback.Models
{
    public class SetPaymentCallbackRequest
    {
        [Required]
        public string MerchantId { get; set; }
        
        [Required]
        public string PaymentRequestId { get; set; }

        [Required]
        public string CallbackUrl { get; set; }
    }
}
