using Microsoft.Azure.ServiceBus;
using ServiceBusCLI.Features.ServiceBus;
using System.IO;

namespace ServiceBusCLI.Utils
{
    public interface IQueueClientAccessor
    {
        QueueClient QueueClient { get; }
    }
    public interface ISessionSettings
    {
        string Namespace { get; }
        string Queue { get; }
    }
    class SessionSettings : ISessionSettings
    {
        public ServiceBusSettings.Settings QueueSettings { get; }
        public string Namespace { get { return QueueSettings.Namespace; } }
        public string Queue { get { return QueueSettings.Queue; } }

        public SessionSettings(AppSettings<Features.ServiceBus.ServiceBusSettings.Settings> appSettings)
        {
            QueueSettings = appSettings.Load(ServiceBusSettings.SettingsFileName);
            if(QueueSettings == null)
            {
                throw new FileNotFoundException($"SessionSettings failed to load.  Make sure you call service-bus-settings");
            }
        }
    }
}