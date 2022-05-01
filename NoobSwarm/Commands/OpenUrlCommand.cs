using Newtonsoft.Json;

using NoobSwarm.Hotkeys;
using NoobSwarm.Makros;
using System.Diagnostics;

namespace NoobSwarm.Commands
{
    public class OpenUrlCommand : IHotkeyCommand
    {
        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }

        public string? Url { get; set; }
        public bool Editable => true;
        public bool Viewable => true;
        public string Content => GetContent();

        public void Execute()
        {
            ProcessStartInfo? startInfo = new (Url)
            {
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }

        public string GetContent() => Url ?? "";
    }
}
