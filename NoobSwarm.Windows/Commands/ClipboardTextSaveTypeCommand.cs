using MessagePack;
using Newtonsoft.Json;
using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Hotkeys;
using NoobSwarm.Makros;
using NoobSwarm.VirtualHID;
using System.Windows.Forms;

namespace NoobSwarm.Windows.Commands
{

    [MessagePackObject]
    public class ClipboardTextSaveTypeCommand : IHotkeyCommand
    {
        [IgnoreMember]
        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }

        [Key(0)]
        public string ClipboardText { get; set; }

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
