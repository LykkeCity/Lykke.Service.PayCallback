using System;
using Autofac;
using Common.Log;

namespace Lykke.Service.PayCallback.Client
{
    public static class AutofacExtension
    {
        public static void RegisterPayCallbackClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<PayCallbackClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<IPayCallbackClient>()
                .SingleInstance();
        }

        public static void RegisterPayCallbackClient(this ContainerBuilder builder, PayCallbackServiceClientSettings settings, ILog log)
        {
            builder.RegisterPayCallbackClient(settings?.ServiceUrl, log);
        }
    }
}
