using System;
using System.Collections.Generic;
using System.Linq;
using TS3QueryLib.Net.Core;
using TS3QueryLib.Net.Core.Common.CommandHandling;
using TS3QueryLib.Net.Core.Common.Notification;

namespace NoobSwarm.Plugin.Ts
{
    internal class ToAdd : INotificationHandler, IDisposable
    {
        private readonly QueryClient client;
        private readonly ClientNotifications notifications;
        private readonly string notificationsString;
        private readonly string[] parameters;
        private readonly uint schandlerid;

        public event EventHandler<IEnumerable<CommandParameter>> On;

        public ToAdd(QueryClient client, ClientNotifications notifications, uint schandlerid, string[] parameters)
        {
            this.client = client;
            this.notifications = notifications;
            this.notificationsString = notifications.GetValueValue();
            this.schandlerid = schandlerid;
            this.parameters = parameters;
        }

        public string Start()
        {
            // register
            Hub.Instance.Add(notifications, this);
            return client.Send($"clientnotifyregister schandlerid={schandlerid} event={notificationsString}");
        }

        public string Stop()
        {
            // unregister
            try
            {
                Hub.Instance.Remove(notifications, this); // Makes no sense, always generating new instance
                return client.Send($"clientnotifyunregister schandlerid={schandlerid} event={notificationsString}");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void HandleResponse(IQueryClient queryClient, string responseText)
        {
            foreach (var parameter in CommandParameterGroupList.Parse(responseText))
            {
                On?.Invoke(this,
                    parameter.Where(x =>
                    parameters?.Contains(x.EncodedName, StringComparer.OrdinalIgnoreCase) != false));
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
