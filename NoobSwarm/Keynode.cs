
using Newtonsoft.Json;
using NoobSwarm.Hotkeys;

using System;
using System.Collections.Generic;
using System.Linq;

using Vulcan.NET;

namespace NoobSwarm
{
    public class KeyNode
    {
        public LedKey Key { get; set; }
        public IHotkeyCommand? Command { get; set; }
        public Dictionary<LedKey, KeyNode> Children { get; set; } = new Dictionary<LedKey, KeyNode>();

        [JsonIgnore]
        public bool HasSinglePath => (
            Children.Count == 0
                && Command != null
            || (Children.Count == 1
                && Command == null
                && Children.First().Value.HasSinglePath));



        [JsonIgnore]
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

        internal bool RemoveNode(Span<LedKey> hotkeys)
        {
            if (hotkeys.Length == 1)
            {
                if (Children.ContainsKey(hotkeys[0]))
                    Children.Remove(hotkeys[0]);
                return Children.Count == 0;
            }

            if (Children.TryGetValue(hotkeys[0], out var child))
            {
                if (child.RemoveNode(hotkeys[1..]))
                {
                    Children.Remove(hotkeys[0]);
                    return Children.Count == 0;
                }
            }
            return false;
        }
    }
}
