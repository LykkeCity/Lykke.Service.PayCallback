using Lykke.Service.PayCallback.Core.Settings.ServiceSettings;
using Lykke.Service.PayCallback.Core.Settings.SlackNotifications;

namespace Lykke.Service.PayCallback.Core.Settings
{
    public class AppSettings
    {
        public PayCallbackSettings PayCallbackService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
