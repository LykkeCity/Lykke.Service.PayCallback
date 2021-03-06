﻿using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.PayCallback.Core.Domain;

namespace Lykke.Service.PayCallback.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class PaymentCallbackEntity : AzureTableEntity
    {
        public static class ByMerchant
        {
            public static string GeneratePartitionKey(string merchantId)
            {
                return merchantId;
            }

            public static string GenerateRowKey(string paymentRequestId)
            {
                return paymentRequestId;
            }

            public static PaymentCallbackEntity Create(IPaymentCallback src)
            {
                return new PaymentCallbackEntity
                {
                    MerchantId = src.MerchantId,
                    PaymentRequestId = src.PaymentRequestId,
                    Url = src.Url,
                    PartitionKey = GeneratePartitionKey(src.MerchantId),
                    RowKey = GenerateRowKey(src.PaymentRequestId)
                };
            }
        }

        public string Id => RowKey;

        public string MerchantId { get; set; }

        public string PaymentRequestId { get; set; }

        public string Url { get; set; }
    }
}
