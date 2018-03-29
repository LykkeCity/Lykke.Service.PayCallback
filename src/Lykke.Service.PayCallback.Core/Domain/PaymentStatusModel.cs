using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Service.PayCallback.Core.Domain
{
    public class PaymentStatusModel
    {
        public string Id { get; set; }

        public string PaymentStatus { get; set; }

        public string OrderId { get; set; }

        public string PaymentAsset { get; set; }

        public string SettlementAsset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull] public PaymentRequestModel PaymentRequest { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull] public RefundRequestModel RefundRequest { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull] public SettlementRequestModel SettlementResponse { get; set; }
    }
}
