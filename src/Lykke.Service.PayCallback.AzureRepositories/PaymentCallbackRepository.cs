using System;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.PayCallback.Core.Domain;

namespace Lykke.Service.PayCallback.AzureRepositories
{
    public class PaymentCallbackRepository : IPaymentCallbackRepository
    {
        private readonly INoSQLTableStorage<PaymentCallbackEntity> _tableStorage;

        public PaymentCallbackRepository(INoSQLTableStorage<PaymentCallbackEntity> tableStorage)
        {
            _tableStorage = tableStorage ?? throw new ArgumentNullException(nameof(tableStorage));
        }

        public async Task<IPaymentCallback> SetAsync(IPaymentCallback paymentCallback)
        {
            var entity = PaymentCallbackEntity.ByMerchant.Create(paymentCallback);

            await _tableStorage.InsertOrMergeAsync(entity);

            return Mapper.Map<PaymentCallback>(entity);
        }

        public async Task<IPaymentCallback> GetAsync(string merchantId, string paymentRequestId)
        {
            PaymentCallbackEntity entity = await _tableStorage.GetDataAsync(
                PaymentCallbackEntity.ByMerchant.GeneratePartitionKey(merchantId),
                PaymentCallbackEntity.ByMerchant.GenerateRowKey(paymentRequestId));

            return Mapper.Map<PaymentCallback>(entity);
        }
    }
}
