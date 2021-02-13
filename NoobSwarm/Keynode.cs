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
        public Action<VulcanKeyboard>? KeineAhnungAction { get; set; }
        public Dictionary<LedKey, KeyNode> Children { get; set; } = new Dictionary<LedKey, KeyNode>();

        public bool HasSinglePath => (
            Children.Count == 0 
                && KeineAhnungAction != null 
            || (Children.Count == 1 
                && KeineAhnungAction == null
                && Children.First().Value.HasSinglePath));

        //Has Children and Action => false
        //Has Children and no Action => "true"
        //Has No Children and Action => true
        

        public KeyNode? SinglePathChild => HasSinglePath ? (Children.Count == 0 ? this : Children.First().Value.SinglePathChild) : null;

    }
}
