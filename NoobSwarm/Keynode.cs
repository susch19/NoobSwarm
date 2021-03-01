using MessagePack;

using NoobSwarm.Hotkeys;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm
{
    [MessagePackObject]
    public class KeyNode
    {
        [Key(0)]
        public LedKey Key { get; set; }
        [Key(1)]
        public IHotkeyCommand? Command { get; set; }
        [Key(2)]
        public Dictionary<LedKey, KeyNode> Children { get; set; } = new Dictionary<LedKey, KeyNode>();

        [IgnoreMember]
        public bool HasSinglePath => (
            Children.Count == 0
                && Command != null
            || (Children.Count == 1
                && Command == null
                && Children.First().Value.HasSinglePath));

        //Has Children and Action => false
        //Has Children and no Action => "true"
        //Has No Children and Action => true


        [IgnoreMember]
        public KeyNode? SinglePathChild => HasSinglePath ? (Children.Count == 0 ? this : Children.First().Value.SinglePathChild) : null;

        internal IEnumerable<(List<LedKey> keys, IHotkeyCommand command)> GetCommands(List<LedKey> keys)
        {
            var localKeys = keys.ToList();
            localKeys.Add(Key);
            if (Command is not null)
                yield return (localKeys, Command);

            foreach (var item in Children)
            {
                foreach (var command in item.Value.GetCommands(localKeys))
                {
                    yield return command;
                }
            }
        }
    }
}
