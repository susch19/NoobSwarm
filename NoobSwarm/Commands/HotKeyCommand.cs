//using Newtonsoft.Json;

//using NonSucking.Framework.Extension.IoC;

//using NoobSwarm.Hotkeys;
//using NoobSwarm.Makros;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using static NoobSwarm.MakroManager;

//namespace NoobSwarm.Commands
//{
//    public class HotKeyCommand : IHotkeyCommand
//    {
//        [JsonIgnore]
//        private IKeyboard? keyboard;

//        [JsonIgnore]
//        public HotKeyType HotKeyType { get; set; }
//        public ushort Makro { get; set; }

//        public HotKeyCommand()
//        {
//        }

//        public HotKeyCommand(ushort makro)
//        {

//            Makro = makro;
//        }
//        public HotKeyCommand(List<RecordKey> makro)
//        {
//            foreach (var item in makro)
//            {
//                if (!item.Pressed)
//                    continue;
//                Makro |= (ushort)item.Key;
//            }
//        }

//        public void Execute()
//        {
//            keyboard ??= TypeContainer.GetOrNull<IKeyboard>();
//            if (keyboard is null)
//                return;

//            if (Makro is not null)
//                keyboard.PlayMacro(Makro);
//        }

//    }
//}
