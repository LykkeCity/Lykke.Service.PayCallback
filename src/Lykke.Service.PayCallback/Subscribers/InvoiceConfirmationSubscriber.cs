using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;
using Lykke.Service.PayCallback.Core.Settings.ServiceSettings;
using System;
using System.Threading.Tasks;
using Lykke.Service.PayCallback.Core.Services;

namespace Lykke.Service.PayCallback.Subscribers
{
    public class InvoiceConfirmationSubscriber : IStartable, IStopable
    {
        private readonly RabbitSettings _settings;
        private readonly ILog _log;
        private RabbitMqSubscriber<InvoiceConfirmation> _subscriber;
        private readonly IInvoiceConfirmationService _service;

        public InvoiceConfirmationSubscriber(RabbitSettings settings,
            IInvoiceConfirmationService service, ILog log)
        {
            _settings = settings;
            _log = log;
            _service = service;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.InvoiceConfirmationExchangeName, "invoiceconfirmation")
                .MakeDurable();

            settings.DeadLetterExchangeName = null;

            var errorHandlingStrategy = new ResilientErrorHandlingStrategy(_log, settings,
                retryTimeout: TimeSpan.FromSeconds(10),
                next: new DeadQueueErrorHandlingStrategy(_log, settings));

            _subscriber = new RabbitMqSubscriber<InvoiceConfirmation>(settings,
                    errorHandlingStrategy)
                .SetMessageDeserializer(new JsonMessageDeserializer<InvoiceConfirmation>())
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

        private Task ProcessMessageAsync(InvoiceConfirmation message)
        {
            return _service.ProcessAsync(message);
        }
    }
}
