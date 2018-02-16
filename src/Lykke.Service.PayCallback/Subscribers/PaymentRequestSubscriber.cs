using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
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
        private readonly ILog _log;
        private RabbitMqSubscriber<PaymentRequestDetailsMessage> _subscriber;

        public PaymentRequestSubscriber(
            ICallbackService callbackService,
            RabbitSettings settings,
            ILog log)
        {
            _callbackService = callbackService;
            _log = log;
            _settings = settings;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.PaymentRequestsExchangeName, "paycallback")
                .MakeDurable();

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<PaymentRequestDetailsMessage>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<PaymentRequestDetailsMessage>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
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
                await _callbackService.ProcessPaymentUpdate(message);

                await _log.WriteInfoAsync(nameof(PaymentRequestSubscriber), nameof(ProcessMessageAsync),
                    message.ToJson(), "Payment request update processed");
            }
            catch (CallbackNotFoundException ex)
            {
                await _log.WriteInfoAsync(nameof(PaymentRequestSubscriber), nameof(ProcessMessageAsync),
                    $"Callback url not found for payment request id {ex.PaymentRequestId}");
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(PaymentRequestSubscriber), nameof(ProcessMessageAsync),
                    message.ToJson(), ex);
                throw;
            }
        }
    }
}
