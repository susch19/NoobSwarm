using Newtonsoft.Json;
using NoobSwarm.Hotkeys;
using NoobSwarm.Makros;
using System.Diagnostics;

namespace NoobSwarm.Commands
{
    public class OpenProgramCommand : IHotkeyCommand
    {
        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }

        public string? Path { get; set; }

        public string? Args { get; set; }
        public bool Editable=> true;
        public bool Viewable=> true;
        public string Content => GetContent();

        public void Execute()
        {
            var startInfo = new ProcessStartInfo(Path);
            startInfo.Arguments = Args ?? string.Empty;
            startInfo.UseShellExecute = Path.EndsWith(".lnk") ? false : true;
            Process.Start(startInfo);
        }
        
        public string GetContent() => $"{Path} {Args}";
    }
}
