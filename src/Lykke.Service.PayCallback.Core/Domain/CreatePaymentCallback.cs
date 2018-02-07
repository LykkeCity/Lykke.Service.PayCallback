namespace Lykke.Service.PayCallback.Core.Domain
{
    public class CreatePaymentCallback : IPaymentCallback
    {
        public string Id { get; set; }

        public string MerchantId { get; set; }

        public string PaymentRequestId { get; set; }

        public string Url { get; set; }
    }
}
