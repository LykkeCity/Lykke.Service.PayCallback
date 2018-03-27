using System;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.PayCallback.Client.Api;
using Lykke.Service.PayCallback.Client.Models;
using Microsoft.Extensions.PlatformAbstractions;
using Refit;

namespace Lykke.Service.PayCallback.Client
{
    public class PayCallbackClient : IPayCallbackClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ICallbacksApi _callbackApi;
        private readonly ApiRunner _runner;

        public PayCallbackClient(PayCallbackServiceClientSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrEmpty(settings.ServiceUrl))
                throw new Exception("Service URL required");

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(settings.ServiceUrl),
                DefaultRequestHeaders =
                {
                    {
                        "User-Agent",
                        $"{PlatformServices.Default.Application.ApplicationName}/{PlatformServices.Default.Application.ApplicationVersion}"
                    }
                }
            };

            _callbackApi = RestService.For<ICallbacksApi>(_httpClient);
            _runner = new ApiRunner();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task SetPaymentCallback(SetPaymentCallbackModel request)
        {
            await _runner.RunAsync(() => _callbackApi.SetPaymentCallback(request));
        }

        public async Task<GetPaymentCallbackModel> GetPaymentCallback(string merchantId, string paymentRequestId)
        {
            return await _runner.RunAsync(() => _callbackApi.GetPaymentCallback(merchantId, paymentRequestId));
        }
    }
}
