namespace Lykke.Service.PayCallback.Models
{
    public class PaymentCallbackResponse
    {
        public string MerchantId { get; set; }

        public string PaymentRequestId { get; set; }

        public string Url { get; set; }
    }
}
