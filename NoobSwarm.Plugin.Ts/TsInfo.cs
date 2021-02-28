using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TS3QueryLib.Net.Core;
using TS3QueryLib.Net.Core.Client.Commands;
using TS3QueryLib.Net.Core.Common.CommandHandling;

namespace NoobSwarm.Plugin.Ts
{
    public class TsInfo : IDisposable
    {
        /// <summary>
        /// My client id (clid)
        /// </summary>
        public uint MyClientID { get; private set; }

        /// <summary>
        /// Events when someone in the channel start or stops talking
        /// </summary>
        public event EventHandler<TalkStatusEventArgs> TalkStatus;

        private readonly QueryClient c;
        private readonly ToAdd talkStatus;

        public TsInfo(string ip = "localhost", ushort port = 25639)
        {
            c = new QueryClient(ip, port, Hub.Instance);

            talkStatus = c.Notification(ClientNotifications.NotifyTalkStatusChange);
            talkStatus.On += TalkStatus_On;
        }

        public async Task Connect(string apiKey)
        {
            await c.ConnectAsync();
            await new AuthCommand(apiKey).ExecuteAsync(c);
            var whoAmI = await new WhoAmICommand().ExecuteAsync(c);
            MyClientID = whoAmI.ClientId;
        }

        public void StartListen()
        {
            talkStatus.Start();
        }

        private void TalkStatus_On(object sender, IEnumerable<CommandParameter> e)
        {
            var clid = Convert.ToUInt32(e.First(x => x.Name == "clid").Value);
            var talk = e.First(x => x.Name == "status").Value == "1";

            TalkStatus?.Invoke(this, new TalkStatusEventArgs(clid, clid == MyClientID, talk));
        }

        public void StopListen()
        {
            talkStatus.Stop();
        }

        public void Disconnect()
        {
            c.Disconnect();
        }

        public void Dispose()
        {
            talkStatus.On -= TalkStatus_On;

            StopListen();
            c.Disconnect();
        }
    }
}
