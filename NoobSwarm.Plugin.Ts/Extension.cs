using System;
using System.Reflection;
using System.Threading;
using TS3QueryLib.Net.Core;

namespace NoobSwarm.Plugin.Ts
{
    internal static class Extension
    {
        public static string GetValueValue(this Enum e)
        {
            var t = e.GetType();
            var name = Enum.GetName(t, e);
            return t.GetField(name).GetCustomAttribute<ValueAttribute>().Value;
        }

        public static ToAdd Notification(this QueryClient client, ClientNotifications notifications, uint schandlerid = 0)
        {
            return new ToAdd(client, notifications, schandlerid, null);
        }

        public static CancellationToken CancelAfter(TimeSpan cancelAfter)
        {
            var cts = new CancellationTokenSource();

            if (cancelAfter != Timeout.InfiniteTimeSpan)
                cts.CancelAfter(cancelAfter);

            return cts.Token;
        }
    }
}
