﻿namespace Lykke.Service.PayCallback.Core.Settings.ServiceSettings
{
    public class PayCallbackSettings
    {
        public DbSettings Db { get; set; }

        public RabbitSettings Rabbit { get; set; }
    }
}
