using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm
{
    public enum HotKeyMode
    {
        /// <summary>
        /// Press the <see cref="HotKeyManager.HotKey"/> once to enter hotkey mode
        /// </summary>
        Passive,

        /// <summary>
        /// Continuously press the <see cref="HotKeyManager.HotKey"/> to stay in the hotkey mode
        /// </summary>
        Active
    }

    public class HotKeyManager : IDisposable
    {
        /// <summary>
        /// The hotkey to controll the hotkey mode
        /// </summary>
        public LedKey HotKey
        {
            get => hotKey;
            set
            {
                IsExecuting = false;
                Mode = HotKeyMode.Active;
                hotKey = value;
            }
        }

        /// <summary>
        /// The color of the <see cref="HotKey"/>
        /// </summary>
        public Color HotKeyColor { get; set; } = Color.Green;

        /// <summary>
        /// The key to execute a available hotkey when multiple hotkeys are available
        /// </summary>
        public LedKey EarlyExitKey { get; set; } = LedKey.ENTER;
        /// <summary>
        /// The key to stop hotkey execution. Will not execute any actions
        /// </summary>
        public LedKey ExitKey { get; set; } = LedKey.ESC;

        /// <summary>
        /// The color of the <see cref="EarlyExitKey"/>
        /// </summary>
        public Color EarlyExitColor { get; set; } = Color.Pink;

        /// <summary>
        /// The color of the <see cref="ExitKey"/>
        /// </summary>
        public Color ExitColor { get; set; } = Color.Red;

        /// <summary>
        /// Are we curently in the hotkey mode and processing keys
        /// </summary>
        public bool IsExecuting
        {
            get => isExecuting;
            private set
            {
                if (isExecuting == value)
                    return;

                if (!isExecuting && value)
                {
                    // Start of hotkey
                    lastColors = keyboard.GetLastSendColorsCopy();
                    SetHotKeysColoring();
                }
                else if (isExecuting && !value)
                {
                    // End of hotkey
                    ResetColor();
                    currentNode = tree;
                }

                Console.Title = value.ToString();
                isExecuting = value;
            }
        }

        /// <summary>
        /// The mode of the hotkey manager
        /// </summary>
        public HotKeyMode Mode { get; set; }

        private readonly VulcanKeyboard keyboard;
        private readonly Tree tree = new();

        private bool isExecuting;
        private LedKey hotKey;
        private byte[] lastColors;
        private KeyNode currentNode;

        public HotKeyManager()
        {
            keyboard = VulcanKeyboard.Initialize();

            keyboard.SetColor(Color.Blue);
            keyboard.Update();

            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;

            currentNode = tree;
            Mode = HotKeyMode.Passive;
        }

        public HotKeyManager(LedKey singleHotKey) : this()
        {
            HotKey = singleHotKey;
            Mode = HotKeyMode.Active;
        }

        /// <summary>
        /// Add a new hotkey to the tree
        /// </summary>
        /// <param name="hotkeys">A list of keys to be pressed for the hotkey execution. Needs atleast one key. First key is used as the hotkey entry, if <see cref="Mode"/> is <see cref="HotKeyMode.Passive"/></param>
        /// <param name="action">Action to be called, when the hotkey is executed</param>
        public void AddHotKey(IReadOnlyList<LedKey> hotkeys, Action<VulcanKeyboard> action)
        {
            if (hotkeys.Count < 1)
                return;

            tree.CreateNode(hotkeys, action);
        }

        private void Keyboard_KeyPressedReceived(object sender, KeyPressedArgs e)
        {

            if (e.Key == HotKey && Mode == HotKeyMode.Active)
            {
                if(!e.IsPressed)
                    currentNode.KeineAhnungAction?.Invoke(keyboard);
                IsExecuting = e.IsPressed;

                // Always return so we dont try to get hotkey child which will not exist
                return;
            }

            if (!e.IsPressed)
                return;
            else if (Mode == HotKeyMode.Passive && !isExecuting)
            {
                if (!tree.Children.ContainsKey(e.Key))
                    return;

                IsExecuting = true;
                hotKey = e.Key;
            }

            if (Mode == HotKeyMode.Passive && (e.Key == EarlyExitKey || e.Key == ExitKey))
            {
                if (e.Key == EarlyExitKey)
                    currentNode.KeineAhnungAction?.Invoke(keyboard);
                IsExecuting = false;
            }

            if (!IsExecuting)
                return;

            if (currentNode.Children.TryGetValue(e.Key, out var nextNode))
            {
                currentNode = nextNode;
                SetHotKeysColoring();

                TestSinglePath(currentNode);
            }
        }
        
    

        private bool TestSinglePath(KeyNode node)
        {
            if (node?.HasSinglePath ?? false)
            {
                Task.Run(() => node.SinglePathChild.KeineAhnungAction?.Invoke(keyboard));
                IsExecuting = false;
                return true;
            }

            return false;
        }

        private void SetHotKeysColoring()
        {
            keyboard.SetColor(Color.Black);

            if (currentNode is not null)
            {
                foreach (var item in currentNode.Children)
                    keyboard.SetKeyColor(item.Key, HotKeyColor);

                if (Mode == HotKeyMode.Passive)
                    keyboard.SetKeyColor(ExitKey, ExitColor);

                if (currentNode.KeineAhnungAction is not null)
                {
                    switch (Mode)
                    {
                        case HotKeyMode.Passive:
                            keyboard.SetKeyColor(EarlyExitKey, EarlyExitColor);
                            break;
                        case HotKeyMode.Active:
                            keyboard.SetKeyColor(HotKey, EarlyExitColor);
                            break;
                    }
                }
            }

            keyboard.Update();
        }

        private void ResetColor()
        {
            if (lastColors is null || lastColors.Length == 0)
                return;

            keyboard.SetColors(lastColors);
            keyboard.Update();
        }

        public void Dispose()
        {
            keyboard.Dispose();
        }
    }
}
