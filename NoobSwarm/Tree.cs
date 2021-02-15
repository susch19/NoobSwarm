﻿using System;
using System.Collections.Generic;

using Vulcan.NET;

namespace NoobSwarm
{
    public class Tree : KeyNode
    {
        private static KeyNode AddOrGetNode(KeyNode parent, LedKey key)
        {
            if (parent.Children.TryGetValue(key, out var node))
                return node;
            node = new KeyNode() { Key = key };
            parent.Children.Add(key, node);
            return node;
        }
        public void CreateNode(IList<LedKey> hotkey, Action<VulcanKeyboard> action)
        {
            KeyNode curNode = this;
            for (int i = 0; i < hotkey.Count; i++)
            {
                var key = hotkey[i];
                var tmpKey = AddOrGetNode(curNode, key);
                //if (tmpKey.KeineAhnungAction != null)
                //{
                //    throw new Exception("node already has an action");
                //}
                curNode = tmpKey;
                if (i == hotkey.Count - 1)
                {
                    tmpKey.KeineAhnungAction = action;
                }
            }
        }
    }
}
