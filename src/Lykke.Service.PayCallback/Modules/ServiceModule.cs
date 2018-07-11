using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common;
using Lykke.Common.Log;
using Lykke.Service.PayCallback.AzureRepositories;
using Lykke.Service.PayCallback.AzureRepositories.InvoiceConfirmation;
using Lykke.Service.PayCallback.Core.Domain;
using Lykke.Service.PayCallback.Core.Domain.InvoiceConfirmation;
using Lykke.Service.PayCallback.Core.Services;
using Lykke.Service.PayCallback.Core.Settings.ServiceSettings;
using Lykke.Service.PayCallback.Services;
using Lykke.Service.PayCallback.Subscribers;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.PayCallback.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<PayCallbackSettings> _settings;
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<PayCallbackSettings> settings)
        {
            _settings = settings;
            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterType<CallbackService>()
                .As<ICallbackService>()
                .SingleInstance();

            builder.Register(c => new PaymentCallbackRepository(
                    AzureTableStorage<PaymentCallbackEntity>.Create(
                        _settings.ConnectionString(x => x.Db.DataConnString),
                        "PaymentCallbacks", c.Resolve<ILogFactory>())))
                .As<IPaymentCallbackRepository>()
                .SingleInstance();

            builder.RegisterType<PaymentRequestSubscriber>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.Rabbit));

            RegisterInvoiceConfirmation(builder);

            builder.Populate(_services);
        }

        private void RegisterInvoiceConfirmation(ContainerBuilder builder)
        {
            builder.RegisterType<InvoiceConfirmationXmlSerializer>()
                .As<IInvoiceConfirmationXmlSerializer>()
                .SingleInstance();

            builder.RegisterType<InvoiceConfirmationService>()
                .As<IInvoiceConfirmationService>()
                .WithParameter("url", _settings.CurrentValue.InvoiceConfirmationUrl)
                .WithParameter("authorization", _settings.CurrentValue.InvoiceConfirmationAuthorization)
                .SingleInstance();

            builder.Register(c => new InvoiceConfirmationRepository(
                    AzureTableStorage<InvoiceConfirmationEntity>.Create(
                        _settings.ConnectionString(x => x.Db.DataConnString),
                        "InvoiceConfirmations", c.Resolve<ILogFactory>())))
                .As<IInvoiceConfirmationRepository>()
                .SingleInstance();

            builder.RegisterType<InvoiceConfirmationSubscriber>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.Rabbit));
        }
    }
}
