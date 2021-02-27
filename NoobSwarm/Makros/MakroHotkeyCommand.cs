using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Hotkeys;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static NoobSwarm.MakroManager;

namespace NoobSwarm.Makros
{
    public class MakroHotkeyCommand : IHotkeyCommand
    {
        private IKeyboard keyboard;

        public HotKeyType HotKeyType { get; set; }
        public List<RecordKey> Makro { get; set; }

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

        public void Deserialize()
        {
        }

        public void Serialize()
        {
        }
    }
}
