using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common;
using Common.Log;
using Lykke.Service.PayCallback.AzureRepositories;
using Lykke.Service.PayCallback.Core.Domain;
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
        private readonly ILog _log;
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<PayCallbackSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

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

            builder.RegisterInstance<IPaymentCallbackRepository>(new PaymentCallbackRepository(
                AzureTableStorage<PaymentCallbackEntity>.Create(_settings.ConnectionString(x => x.Db.DataConnString),
                    "PaymentCallbacks", _log)));

            builder.RegisterType<PaymentRequestSubscriber>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.Rabbit));

            builder.Populate(_services);
        }
    }
}
