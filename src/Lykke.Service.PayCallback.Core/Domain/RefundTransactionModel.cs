﻿using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Service.PayCallback.Core.Domain
{
    public class RefundTransactionModel
    {
        public string TransactionId { get; set; }

        public string Timestamp { get; set; }

        public int NumberOfConfirmations { get; set; }

        public decimal Amount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [CanBeNull] public string Url { get; set; }
    }
}
