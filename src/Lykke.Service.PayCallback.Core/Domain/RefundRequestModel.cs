using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Service.PayCallback.Core.Domain
{
    public class RefundRequestModel
    {
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        public decimal Amount { get; set; }

        public string Address { get; set; }

        [JsonProperty("expiration_datetime")]
        public string ExpirationDt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull] public string Error { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull] public IList<RefundTransactionModel> Transactions { get; set; }
    }
}
