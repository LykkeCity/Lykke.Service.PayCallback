namespace Lykke.Service.PayCallback.Client.Models
{
    public class SetPaymentCallbackModel
    {
        public string MerchantId { get; set; }
        
        public string PaymentRequestId { get; set; }

        public string CallbackUrl { get; set; }
    }
}
