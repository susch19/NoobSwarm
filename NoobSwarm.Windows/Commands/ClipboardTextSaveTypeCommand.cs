using Newtonsoft.Json;
using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Hotkeys;
using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;
using System.Windows.Forms;

namespace NoobSwarm.Windows.Commands
{

    public class ClipboardTextSaveTypeCommand : IHotkeyCommand
    {
        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }

        public string? ClipboardText { get; set; }

        private Keyboard? keyboard;

        public void GetDataFromClipboard()
        {
            ClipboardText = Clipboard.GetText();
        }

        public void Execute()
        {
            keyboard ??= TypeContainer.GetOrNull<Keyboard>();
            if (keyboard is null)
                return;

            keyboard.SendCharsSequene(ClipboardText);

        }
    }
}
