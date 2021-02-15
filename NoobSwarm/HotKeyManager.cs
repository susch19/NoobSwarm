using System;
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
        /// The color of the <see cref="EarlyExitKey"/>
        /// </summary>
        public Color EarlyExitColor { get; set; } = Color.Pink;
        
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
                    currentNode?.KeineAhnungAction?.Invoke(keyboard);

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
        private LedKey hotKey = LedKey.FN_Key;
        private byte[] lastColors;
        private KeyNode currentNode;

        private static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }

        public HotKeyManager()
        {
            keyboard = VulcanKeyboard.Initialize();

            keyboard.SetColor(Color.Blue);
            keyboard.Update();

            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;

            tree.CreateNode(new[] { LedKey.P }, x => Console.WriteLine("Toggle"));
            tree.CreateNode(new[] { LedKey.P, LedKey.L }, x => Console.WriteLine("Play"));
            tree.CreateNode(new[] { LedKey.P, LedKey.P }, x => Console.WriteLine("Pause"));
            tree.CreateNode(new[] { LedKey.T, LedKey.W }, x => OpenUrl("https://www.twitch.tv/"));
            tree.CreateNode(new[] { LedKey.T, LedKey.W, LedKey.N }, x => OpenUrl("https://www.twitch.tv/noobdevtv"));
            currentNode = tree;
        }

        private void Keyboard_KeyPressedReceived(object sender, KeyPressedArgs e)
        {
            if (e.Key == HotKey)
            {
                switch (Mode)
                {
                    case HotKeyMode.Passive:
                        IsExecuting = true;
                        break;

                    case HotKeyMode.Active:
                        IsExecuting = e.IsPressed;
                        break;
                }

                // Always return so we dont try to get hotkey child which will not exist
                return;
            }

            if (Mode == HotKeyMode.Passive && e.Key == EarlyExitKey)
                IsExecuting = false;

            if (!IsExecuting || !e.IsPressed)
                return;

            if (!TestSinglePath(currentNode) && currentNode is not null)
            {
                currentNode.Children.TryGetValue(e.Key, out currentNode);
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
