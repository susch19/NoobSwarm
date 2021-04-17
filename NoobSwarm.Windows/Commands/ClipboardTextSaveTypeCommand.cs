using Newtonsoft.Json;
using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Hotkeys;
using NoobSwarm.Makros;

using TextCopy;

namespace NoobSwarm.Windows.Commands
{

    public class ClipboardTextSaveTypeCommand : IHotkeyCommand
    {
        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }

        public string? ClipboardText { get; set; }

        private IKeyboard? keyboard;

        public void GetDataFromClipboard()
        {
            ClipboardText = ClipboardService.GetText();
        }

        public void Execute()
        {
            keyboard ??= TypeContainer.GetOrNull<IKeyboard>();
            if (keyboard is null)
                return;

            keyboard.SendCharsSequene(ClipboardText);

        }
    }
}
