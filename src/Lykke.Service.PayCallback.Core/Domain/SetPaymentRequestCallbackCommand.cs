namespace Lykke.Service.PayCallback.Core.Domain
{
    public class SetPaymentRequestCallbackCommand
    {
        public string MerchantId { get; set; }
        
        public string PaymentRequestId { get; set; }

        public string CallbackUrl { get; set; }
    }
}
