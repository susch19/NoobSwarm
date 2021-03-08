using Newtonsoft.Json;
using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Hotkeys;
using System.Collections.Generic;

using static NoobSwarm.MakroManager;

namespace NoobSwarm.Makros
{
    public class MakroHotkeyCommand : IHotkeyCommand
    {
        [JsonIgnore]
        private IKeyboard? keyboard;

        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }
        public List<RecordKey>? Makro { get; set; }

        public MakroHotkeyCommand()
        {
        }

        public MakroHotkeyCommand(List<RecordKey> makro)
        {
            Makro = makro;
        }

        public void Execute()
        {
            keyboard ??= TypeContainer.GetOrNull<IKeyboard>();
            if (keyboard is null)
            {
                return;
            }

            if (Makro is not null)
                keyboard.PlayMacro(Makro);
        }

    }
}
