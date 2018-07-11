using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    public class InvoiceConfirmationPublisher : IStartable, IStopable
    {
        private readonly RabbitMqPublisherSettings _settings;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;
        private RabbitMqPublisher<IInvoiceConfirmation> _publisher;

        [Obsolete]
        public InvoiceConfirmationPublisher(RabbitMqPublisherSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        public InvoiceConfirmationPublisher(RabbitMqPublisherSettings settings, 
            ILogFactory logFactory)
        {
            _settings = settings;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);

        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings.CreateForPublisher(_settings.ConnectionString, _settings.ExchangeName);
            settings.MakeDurable();

            if (_logFactory == null)
            {
                _publisher = new RabbitMqPublisher<IInvoiceConfirmation>(settings)
                    .SetConsole(new LogToConsole())
                    .SetLogger(_log);
            }
            else
            {
                _publisher = new RabbitMqPublisher<IInvoiceConfirmation>(_logFactory, settings);
            }

            _publisher.DisableInMemoryQueuePersistence()
                .PublishSynchronously()
                .SetSerializer(new JsonMessageSerializer<IInvoiceConfirmation>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .Start();

            _log.Info($"<< {nameof(InvoiceConfirmationPublisher)} is started.");
        }

        public async Task PublishAsync(IInvoiceConfirmation invoiceConfirmation)
        {
            Validate(invoiceConfirmation);

            await _publisher.ProduceAsync(invoiceConfirmation);

            _log.Info("Invoice confirmation is published.", invoiceConfirmation.ToJson());
        }

        protected virtual void Validate(IInvoiceConfirmation invoiceConfirmation)
        {
            var context = new ValidationContext(invoiceConfirmation);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(invoiceConfirmation, context, results, true);

            if (!isValid)
            {
                var modelErrors = new Dictionary<string, List<string>>();
                foreach (ValidationResult validationResult in results)
                {
                    if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                    {
                        foreach (string memberName in validationResult.MemberNames)
                        {
                            AddModelError(modelErrors, memberName, validationResult.ErrorMessage);
                        }
                    }
                    else
                    {
                        AddModelError(modelErrors, string.Empty, validationResult.ErrorMessage);
                    }
                }

                throw new InvoiceConfirmationException("Model is invalid.", modelErrors);
            }

            if (invoiceConfirmation.InvoiceList != null)
            {
                foreach (var invoiceOperation in invoiceConfirmation.InvoiceList)
                {
                    Validate(invoiceOperation);
                }
            }

            Validate(invoiceConfirmation.CashOut);
        }

        protected virtual void Validate(InvoiceOperation invoiceOperation)
        {
            var context = new ValidationContext(invoiceOperation);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(invoiceOperation, context, results, true);

            if (!isValid)
            {
                var modelErrors = new Dictionary<string, List<string>>();
                foreach (ValidationResult validationResult in results)
                {
                    if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                    {
                        foreach (string memberName in validationResult.MemberNames)
                        {
                            AddModelError(modelErrors, memberName, validationResult.ErrorMessage);
                        }
                    }
                    else
                    {
                        AddModelError(modelErrors, string.Empty, validationResult.ErrorMessage);
                    }
                }

                throw new InvoiceConfirmationException("Model is invalid.", modelErrors);
            }

            Validate(invoiceOperation.Dispute);
        }

        protected virtual void Validate(DisputeOperation disputeOperation)
        {
            if (disputeOperation == null)
            {
                return;
            }

            var context = new ValidationContext(disputeOperation);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(disputeOperation, context, results, true);

            if (!isValid)
            {
                var modelErrors = new Dictionary<string, List<string>>();
                foreach (ValidationResult validationResult in results)
                {
                    if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                    {
                        foreach (string memberName in validationResult.MemberNames)
                        {
                            AddModelError(modelErrors, memberName, validationResult.ErrorMessage);
                        }
                    }
                    else
                    {
                        AddModelError(modelErrors, string.Empty, validationResult.ErrorMessage);
                    }
                }

                throw new InvoiceConfirmationException("Model is invalid.", modelErrors);
            }
        }

        protected virtual void Validate(CashOut cashOut)
        {
            if (cashOut == null)
            {
                return;
            }

            var context = new ValidationContext(cashOut);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(cashOut, context, results, true);

            if (!isValid)
            {
                var modelErrors = new Dictionary<string, List<string>>();
                foreach (ValidationResult validationResult in results)
                {
                    if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                    {
                        foreach (string memberName in validationResult.MemberNames)
                        {
                            AddModelError(modelErrors, memberName, validationResult.ErrorMessage);
                        }
                    }
                    else
                    {
                        AddModelError(modelErrors, string.Empty, validationResult.ErrorMessage);
                    }
                }

                throw new InvoiceConfirmationException("Model is invalid.", modelErrors);
            }
        }

        private void AddModelError(Dictionary<string, List<string>> modelErrors, string memberName,
            string errorMessage)
        {
            if (!modelErrors.TryGetValue(memberName, out var errors))
            {
                errors = new List<string>();
                modelErrors[memberName] = errors;
            }

            errors.Add(errorMessage);
        }

        public void Dispose()
        {
            _publisher?.Dispose();
        }

        public void Stop()
        {
            _publisher?.Stop();
            _log.Info($"<< {nameof(InvoiceConfirmationPublisher)} is stopped.");
        }
    }
}
