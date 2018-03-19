namespace Lykke.Service.PayCallback.Core.Domain
{
    public class PaymentCallback : IPaymentCallback
    {
        public string Id { get; }

        public string MerchantId { get; set; }

        public string PaymentRequestId { get; set; }

        public string Url { get; set; }
    }
}
