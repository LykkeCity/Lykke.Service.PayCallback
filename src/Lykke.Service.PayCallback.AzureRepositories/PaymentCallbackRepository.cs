using System;
using System.Threading.Tasks;
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

        public async Task<IPaymentCallback> InsertAsync(IPaymentCallback paymentCallback)
        {
            var entity = PaymentCallbackEntity.ByMerchant.Create(paymentCallback);

            await _tableStorage.InsertAsync(entity);

            return entity;
        }

        public async Task<IPaymentCallback> GetAsync(IPaymentCallback paymentCallback)
        {
            return await _tableStorage.GetDataAsync(
                PaymentCallbackEntity.ByMerchant.GeneratePartitionKey(paymentCallback.MerchantId),
                PaymentCallbackEntity.ByMerchant.GenerateRowKey(paymentCallback.PaymentRequestId));
        }
    }
}
