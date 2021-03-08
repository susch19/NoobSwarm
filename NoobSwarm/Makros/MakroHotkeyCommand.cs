using MessagePack;
using Newtonsoft.Json;
using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Hotkeys;
using System.Collections.Generic;

using static NoobSwarm.MakroManager;

namespace NoobSwarm.Makros
{
    [MessagePackObject]
    public class MakroHotkeyCommand : IHotkeyCommand
    {
        [IgnoreMember]
        [JsonIgnore]
        private IKeyboard? keyboard;

        [IgnoreMember]
        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }
        [Key(0)]
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
