using Autofac;
using Common;
using Common.Log;
using Lykke.Service.PayCallback.Client.InvoiceConfirmation;

namespace Lykke.Service.PayCallback.Client
{
    public static class AutofacExtension
    {
        public static void RegisterInvoiceConfirmationPublisher(this ContainerBuilder builder,
            RabbitMqPublisherSettings settings, ILog log = null)
        {
            var registration = builder.RegisterType<InvoiceConfirmationPublisher>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("settings", settings);

            if (log != null)
            {
                registration.WithParameter("log", log);
            }
        }
    }
}
