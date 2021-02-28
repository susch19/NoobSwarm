using TS3QueryLib.Net.Core.Common.Notification;

namespace NoobSwarm.Plugin.Ts
{
    internal class Hub : NotificationHubBase
    {
        private static Hub instance;
        public static Hub Instance => instance ??= new();

        public bool Add(ClientNotifications notifications, INotificationHandler handler)
        {
            return AddHandler(notifications == ClientNotifications.Any ? "*" : notifications.GetValueValue(), handler);
        }

        public bool Remove(ClientNotifications notifications, INotificationHandler handler)
        {
            return RemoveHandler(notifications == ClientNotifications.Any ? "*" : notifications.GetValueValue(), handler);
        }
    }
}
