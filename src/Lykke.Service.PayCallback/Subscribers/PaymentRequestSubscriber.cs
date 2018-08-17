using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.PayCallback.Core.Exceptions;
using Lykke.Service.PayCallback.Core.Services;
using Lykke.Service.PayCallback.Core.Settings.ServiceSettings;
using Lykke.Service.PayInternal.Contract.PaymentRequest;

namespace Lykke.Service.PayCallback.Subscribers
{
    public class PaymentRequestSubscriber : IStartable, IStopable
    {
        private readonly RabbitSettings _settings;
        private readonly ICallbackService _callbackService;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;
        private RabbitMqSubscriber<PaymentRequestDetailsMessage> _subscriber;

        public PaymentRequestSubscriber(
            ICallbackService callbackService,
            RabbitSettings settings,
            ILogFactory logFactory)
        {
            _callbackService = callbackService;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
            _settings = settings;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.PaymentRequestsExchangeName, "paycallback")
                .MakeDurable();

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<PaymentRequestDetailsMessage>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<PaymentRequestDetailsMessage>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber.Stop();
        }

        private async Task ProcessMessageAsync(PaymentRequestDetailsMessage message)
        {
            try
            {
                await _callbackService.ProcessPaymentRequestUpdate(message);

                _log.Info("Payment request update processed", message.ToJson());
            }
            catch (CallbackNotFoundException ex)
            {
                _log.Info($"Callback url not found for payment request id {ex.PaymentRequestId}");
            }
            catch (Exception ex)
            {
                _log.Error(ex, null, message.ToJson());
                throw;
            }
        }
    }
}
