﻿using MessagePack;

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
    public class OpenProgramCommand : IHotkeyCommand
    {
        [IgnoreMember]
        public HotKeyType HotKeyType { get; set; }

        [Key(0)]
        public string Path { get; set; }

        [Key(2)]
        public string Args { get; set; }

        public void Execute()
        {
            var startInfo = new ProcessStartInfo(Path);
            startInfo.Arguments = Args ?? string.Empty;
            startInfo.UseShellExecute = Path.EndsWith(".lnk") ? false : true;
            Process.Start(startInfo);
        }
    }
}
