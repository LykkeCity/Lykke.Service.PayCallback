using Lykke.Service.PayCallback.Core.Settings.ServiceSettings;
using Lykke.Service.PayCallback.Core.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayCallback.Core.Settings
{
    public class AppSettings
    {
        public PayCallbackSettings PayCallbackService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }

    public class MonitoringServiceClientSettings
    {
        [HttpCheck("api/isalive", false)]
        public string MonitoringServiceUrl { get; set; }
    }
}
