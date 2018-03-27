namespace Lykke.Service.PayCallback.Client.Models
{
    public class GetPaymentCallbackModel
    {
        public string MerchantId { get; set; }

        public string PaymentRequestId { get; set; }

        public string Url { get; set; }
    }
}
