using MessagePack;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Hotkeys;
using NoobSwarm.Makros;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NoobSwarm.Commands
{
    [MessagePackObject]
    public class OpenUrlCommand : IHotkeyCommand
    {
        [IgnoreMember]
        public HotKeyType HotKeyType { get; set; }

        [Key(0)]
        public string Url { get; set; }

        public void Execute()
        {
            ProcessStartInfo? startInfo = new (Url)
            {
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }
    }
}
