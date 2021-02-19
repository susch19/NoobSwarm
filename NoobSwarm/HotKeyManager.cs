using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
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

    public class HotKeyManager
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
                    BackUpColor();
                    SetHotKeysColoring();
                }
                else if (isExecuting && !value)
                {
                    // End of hotkey
                    RestoreColor();
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

        public Color RecordingColorPrimary { get; set; } = Color.DarkGreen;
        public Color RecordingColorSecondary { get; set; } = Color.Yellow;

        private readonly VulcanKeyboard keyboard;
        private readonly Tree tree = new();

        private bool isExecuting;
        private LedKey hotKey;
        private byte[] lastColors;
        private KeyNode currentNode;

        // Synchron recording variables
        private bool isSynchronRecording;
        private List<LedKey> synchronRecordingKeys = new();
        private AutoResetEvent synchronRecordingResetEvent = new(false);

        private bool isAsyncRecording;
        private TaskCompletionSource<LedKey> asyncTaskCompletionSource;
        private CancellationTokenSource asyncToken;
        private LedKey? lastAsyncRecordingKey;

        public HotKeyManager(VulcanKeyboard keyboard)
        {
            this.keyboard = keyboard;
            keyboard.SetColor(Color.Blue);
            keyboard.Update();

            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;
            keyboard.VolumeKnobTurnedReceived += Keyboard_VolumeKnobTurnedReceived;

            currentNode = tree;
            Mode = HotKeyMode.Passive;
        }

        private void Keyboard_VolumeKnobTurnedReceived(object sender, VolumeKnDirectionArgs e)
        {
            if (isSynchronRecording)
                synchronRecordingResetEvent.Set();
            else if (isAsyncRecording)
                asyncToken?.Cancel();
        }

        public HotKeyManager(VulcanKeyboard keyboard, LedKey singleHotKey) : this(keyboard)
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

        /// <summary>
        /// Records all <see cref="LedKey"/> pressed till <see cref="VulcanKeyboard.VolumeKnobTurnedReceived"/> is received
        /// </summary>
        public ReadOnlyCollection<LedKey> RecordKeys()
        {
            synchronRecordingKeys.Clear();
            BackUpColor();
            SetColor(Color.Black);
            isSynchronRecording = true;

            synchronRecordingResetEvent.WaitOne();
            isSynchronRecording = false;
            RestoreColor();

            return new ReadOnlyCollection<LedKey>(synchronRecordingKeys);
        }

        /// <summary>
        /// Async ecording of all pressed keys until <paramref name="token"/> is cancelled or
        /// <see cref="VulcanKeyboard.VolumeKnobTurnedReceived"/> is received
        /// </summary>
        /// <param name="token">Token to cancel the recording</param>
        /// <returns>async enumerable which contains the recording keys</returns>
        public async IAsyncEnumerable<LedKey> Record([EnumeratorCancellation] CancellationToken token)
        {
            asyncToken = CancellationTokenSource.CreateLinkedTokenSource(token);
            asyncToken.Token.Register(() => asyncTaskCompletionSource?.TrySetCanceled());

            BackUpColor();
            SetColor(Color.Black);
            isAsyncRecording = true;
            try
            {
                while (!asyncToken.Token.IsCancellationRequested)
                {
                    asyncTaskCompletionSource = new TaskCompletionSource<LedKey>(TaskCreationOptions.AttachedToParent);
                    yield return await asyncTaskCompletionSource.Task;
                }
            }
            finally
            {
                isAsyncRecording = false;
                RestoreColor();
            }
        }

        private void Keyboard_KeyPressedReceived(object sender, KeyPressedArgs e)
        {
            if (isSynchronRecording || isAsyncRecording)
            {
                if (e.IsPressed)
                {
                    keyboard.SetColor(Color.Black);


                    if (isSynchronRecording)
                    {
                        if (synchronRecordingKeys.Count > 0)
                            keyboard.SetKeyColor(synchronRecordingKeys[^1], RecordingColorSecondary);

                        synchronRecordingKeys.Add(e.Key);
                    }
                    else if (isAsyncRecording)
                    {
                        if (lastAsyncRecordingKey is not null)
                            keyboard.SetKeyColor(lastAsyncRecordingKey.Value, RecordingColorSecondary);

                        lastAsyncRecordingKey = e.Key;
                        asyncTaskCompletionSource?.SetResult(e.Key);
                    }

                    keyboard.SetKeyColor(e.Key, RecordingColorPrimary);
                    keyboard.Update();

                }

                return;
            }

            var lastNode = currentNode;
            if (e.Key == HotKey && Mode == HotKeyMode.Active)
            {
                IsExecuting = e.IsPressed;
                if (!e.IsPressed)
                    lastNode.KeineAhnungAction?.Invoke(keyboard);

                // Always return so we dont try to get hotkey child which will not exist
                return;
            }

            if (!e.IsPressed)
                return;

            if (Mode == HotKeyMode.Passive && !isExecuting)
            {
                if (!tree.Children.ContainsKey(e.Key))
                    return;

                IsExecuting = true;
                hotKey = e.Key;
            }

            if (Mode == HotKeyMode.Passive && (e.Key == EarlyExitKey || e.Key == ExitKey))
            {
                IsExecuting = false;
                if (e.Key == EarlyExitKey)
                    lastNode.KeineAhnungAction?.Invoke(keyboard);
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

        private void BackUpColor()
        {
            lastColors = keyboard.GetLastSendColorsCopy();
        }

        private void RestoreColor()
        {
            if (lastColors is null || lastColors.Length == 0)
                return;

            keyboard.SetColors(lastColors);
            keyboard.Update();
        }

        private void SetColor(Color color)
        {
            keyboard.SetColor(color);
            keyboard.Update();
        }
    }
}
