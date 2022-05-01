using Newtonsoft.Json;
using NonSucking.Framework.Extension.IoC;
using System.Linq;

using NoobSwarm.Hotkeys;
using System.Collections.Generic;

using static NoobSwarm.MakroManager;
using System;

namespace NoobSwarm.Makros
{
    public class MakroHotkeyCommand : IHotkeyCommand
    {
        [JsonIgnore]
        private IKeyboard? keyboard;

        [JsonIgnore]
        public HotKeyType HotKeyType { get; set; }
        public List<RecordKey>? Makro { get; set; }
        public bool Editable { get; }
        public bool Viewable => true;
        public string Content => GetContent();

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

        public string GetContent()
        {
            var data = string.Join(Environment.NewLine, Makro?.Select(x => $"Key={x.Key}, Delay={x.TimeBeforePress}, Down={x.Pressed}") ?? Array.Empty<string>());
            return data;
        }
    }
}
