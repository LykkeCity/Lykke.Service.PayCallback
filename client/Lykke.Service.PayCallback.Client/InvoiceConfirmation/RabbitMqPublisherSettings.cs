using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayCallback.Client.InvoiceConfirmation
{
    public class RabbitMqPublisherSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string ExchangeName { get; set; }
    }
}
