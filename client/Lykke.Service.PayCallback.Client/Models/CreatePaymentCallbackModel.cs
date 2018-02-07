namespace Lykke.Service.PayCallback.Client.Models
{
    public class CreatePaymentCallbackModel
    {
        public string MerchantId { get; set; }
        
        public string PaymentRequestId { get; set; }

        public string CallbackUrl { get; set; }
    }
}
