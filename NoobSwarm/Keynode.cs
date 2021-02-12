using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm
{
    public class KeyNode
    {
        public LedKey Key { get; set; }
        public Action? KeineAhnungAction { get; set; }
        public Dictionary<LedKey, KeyNode> Children { get; set; } = new Dictionary<LedKey, KeyNode>();

        public bool HasSinglePath => Children.Count == 0 || (Children.Count == 1 && Children.First().Value.HasSinglePath);
        public KeyNode? SinglePathChild => Children.Count == 0 ? this : Children.First().Value.SinglePathChild;

    }
}
