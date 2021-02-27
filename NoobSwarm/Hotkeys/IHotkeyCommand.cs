using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.Hotkeys
{
    public interface IHotkeyCommand
    {
        HotKeyType HotKeyType { get; set; }
        
        void Execute();
        void Serialize();
        void Deserialize();
    }
}
