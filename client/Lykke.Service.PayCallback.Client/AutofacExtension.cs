using System;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.PayCallback.Client.InvoiceConfirmation;

namespace Lykke.Service.PayCallback.Client
{
    public static class AutofacExtension
    {
        public static void RegisterInvoiceConfirmationPublisher(this ContainerBuilder builder,
            RabbitMqPublisherSettings settings)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            builder.RegisterType<InvoiceConfirmationPublisher>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .UsingConstructor(typeof(RabbitMqPublisherSettings), typeof(ILogFactory))
                .WithParameter("settings", settings);
        }

        [Obsolete]
        public static void RegisterInvoiceConfirmationPublisher(this ContainerBuilder builder,
            RabbitMqPublisherSettings settings, ILog log)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            builder.RegisterType<InvoiceConfirmationPublisher>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .UsingConstructor(typeof(RabbitMqPublisherSettings), typeof(ILog))
                .WithParameter("settings", settings)
                .WithParameter("log", log);
        }
    }
}
