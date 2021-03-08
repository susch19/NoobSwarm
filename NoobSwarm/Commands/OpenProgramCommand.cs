using MessagePack;
using Newtonsoft.Json;
using NoobSwarm.Hotkeys;
using NoobSwarm.Makros;
using System.Diagnostics;

namespace NoobSwarm.Commands
{
    [MessagePackObject]
    public class OpenProgramCommand : IHotkeyCommand
    {
        [IgnoreMember]
        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }

        [Key(0)]
        public string Path { get; set; }

        [Key(2)]
        public string Args { get; set; }

        public void Execute()
        {
            var startInfo = new ProcessStartInfo(Path);
            startInfo.Arguments = Args ?? string.Empty;
            startInfo.UseShellExecute = Path.EndsWith(".lnk") ? false : true;
            Process.Start(startInfo);
        }
    }
}
