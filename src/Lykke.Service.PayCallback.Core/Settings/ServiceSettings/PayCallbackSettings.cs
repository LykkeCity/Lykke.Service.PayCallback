namespace Lykke.Service.PayCallback.Core.Settings.ServiceSettings
{
    public class PayCallbackSettings
    {
        public DbSettings Db { get; set; }

        public RabbitSettings Rabbit { get; set; }

        public string InvoiceConfirmationUrl { get; set; }

        public string InvoiceConfirmationAuthorization { get; set; }
    }
}
