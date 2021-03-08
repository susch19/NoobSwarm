using MessagePack;
using Newtonsoft.Json;

using NoobSwarm.Hotkeys;
using NoobSwarm.Makros;
using System.Diagnostics;

namespace NoobSwarm.Commands
{
    [MessagePackObject]
    public class OpenUrlCommand : IHotkeyCommand
    {
        [IgnoreMember]
        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }

        [Key(0)]
        public string Url { get; set; }

        public void Execute()
        {
            ProcessStartInfo? startInfo = new (Url)
            {
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }
    }
}
