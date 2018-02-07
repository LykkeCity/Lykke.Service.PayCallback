namespace Lykke.Service.PayCallback.Core.Domain
{
    public interface IPaymentCallback
    {
        string Id { get; }

        string MerchantId { get; set; }

        string PaymentRequestId { get; set; }

        string Url { get; set; }
    }
}
