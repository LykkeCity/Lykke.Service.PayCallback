using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayCallback.Core.Settings.ServiceSettings
{
    public class RabbitSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }
        
        public string PaymentRequestsExchangeName { get; set; }
    }
}
