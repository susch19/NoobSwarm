using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

using NonSucking.Framework.Extension.IoC;

using NoobSwarm.Hotkeys;
using NoobSwarm.Lights;
using NoobSwarm.Lights.LightEffects;
using NoobSwarm.Lights.LightEffects.Wrapper;
using NoobSwarm.Serializations;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
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
        public event EventHandler StartedHotkeyMode;
        public event EventHandler StoppedHotkeyMode;

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

        [JsonIgnore]
        [IgnoreDataMember]
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
                    StartedHotkeyMode?.Invoke(this, new());
                    SetHotKeysColoring();
                    AddHotKeyEffect();
                }
                else if (isExecuting && !value)
                {
                    // End of hotkey
                    RemoveHotKeyEffect();
                    currentNode = tree;
                    StoppedHotkeyMode?.Invoke(this, new());
                    Debug.WriteLine("Stop execution");
                }

                //Console.Title = value.ToString();
                isExecuting = value;
            }
        }

        /// <summary>
        /// The mode of the hotkey manager
        /// </summary>
        public HotKeyMode Mode { get; set; }

        public Color RecordingColorPrimary { get; set; } = Color.DarkGreen;
        public Color RecordingColorSecondary { get; set; } = Color.Yellow;

        [JsonProperty]
        private Tree tree = new();
        private readonly VulcanKeyboard keyboard;
        private bool isExecuting;
        private LedKey hotKey;
        private LightService lightService;
        //private SingleKeysColorEffect hotKeyEffect;
        private PerKeyLightEffectWrapper hotKeyEffect;
        private PerKeyLightEffectWrapper earlyExitEffect;
        private LightEffectWrapper solidBlackEffect;

        //private BreathingColorPerKeyEffect breathingHotKeyEffect;
        private KeyNode currentNode;
        private HashSet<LedKey> ledKeys = new();
        private HashSet<LedKey> exitKeys = new();
        private bool isSynchronRecording;
        private List<LedKey> synchronRecordingKeys = new();
        private SemaphoreSlim synchronRecordingSemaphore = new(0);
        private bool isAsyncRecording;
        private TaskCompletionSource<LedKey>? asyncTaskCompletionSource;
        private CancellationTokenSource? asyncToken;
        private LedKey? lastAsyncRecordingKey;



        public HotKeyManager(VulcanKeyboard keyboard, LightService lightService)
        {
            this.keyboard = keyboard;
            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;
            keyboard.VolumeKnobTurnedReceived += Keyboard_VolumeKnobTurnedReceived;

            this.lightService = lightService;
            hotKeyEffect =
                new PerKeyLightEffectWrapper(new(), new ColorizeLightEffectWrapper(
                new BreathingColorEffect(), new SolidColorEffect(HotKeyColor)
                ));
            earlyExitEffect =
                new PerKeyLightEffectWrapper(new(), new ColorizeLightEffectWrapper(
                new BreathingColorEffect(), new SolidColorEffect(EarlyExitColor)
                ));
            solidBlackEffect = new LightEffectWrapper(new SolidColorEffect(Color.Black));

            //hotKeyEffect = new SingleKeysColorEffect(new(), Color.Black);
            //breathingHotKeyEffect = new  BreathingColorPerKeyEffect(ledColors) { Speed = 20 };

            currentNode = tree;
            Mode = HotKeyMode.Passive;
        }

        public HotKeyManager()
        {
            keyboard = TypeContainer.Get<VulcanKeyboard>();
            keyboard.KeyPressedReceived += Keyboard_KeyPressedReceived;
            keyboard.VolumeKnobTurnedReceived += Keyboard_VolumeKnobTurnedReceived;
            lightService = TypeContainer.Get<LightService>();
        }


        public HotKeyManager(VulcanKeyboard keyboard, LightService lightService, LedKey singleHotKey) : this(keyboard, lightService)
        {
            HotKey = singleHotKey;
            Mode = HotKeyMode.Active;
        }

        public void Serialize()
        {
            using var fs = File.OpenWrite("Makros.save");
            using var writer = new BsonDataWriter(fs);
            SerializationHelper.TypeSafeSerializer.Serialize(writer, this);
        }
        public static HotKeyManager Deserialize()
        {
            if (!File.Exists("Makros.save"))
                return TypeContainer.CreateObject<HotKeyManager>();

            using var fs = File.OpenRead("Makros.save");
            using var reader = new BsonDataReader(fs);

            var hkm = SerializationHelper.TypeSafeSerializer.Deserialize<HotKeyManager>(reader) ?? TypeContainer.CreateObject<HotKeyManager>();

            hkm.hotKeyEffect =
              new PerKeyLightEffectWrapper(hkm.ledKeys, new ColorizeLightEffectWrapper(
              new BreathingColorEffect(), new SolidColorEffect(hkm.HotKeyColor)
              ));
            hkm.earlyExitEffect =
                new PerKeyLightEffectWrapper(hkm.exitKeys, new ColorizeLightEffectWrapper(
                new BreathingColorEffect(), new SolidColorEffect(hkm.EarlyExitColor)
                ));
            hkm.solidBlackEffect = new LightEffectWrapper(new SolidColorEffect(Color.Black));
            hkm.currentNode = hkm.tree;

            return hkm;
        }

        private void Keyboard_VolumeKnobTurnedReceived(object? sender, VolumeKnDirectionArgs e)
        {
            if (isSynchronRecording)
                synchronRecordingSemaphore.Release();
            else if (isAsyncRecording)
                asyncToken?.Cancel();
        }
        /// <summary>
        /// Add a new hotkey to the tree
        /// </summary>
        /// <param name="hotkeys">A list of keys to be pressed for the hotkey execution. Needs atleast one key. First key is used as the hotkey entry, if <see cref="Mode"/> is <see cref="HotKeyMode.Passive"/></param>
        /// <param name="command">Command to be called, when the hotkey is executed</param>
        public void AddHotKey(IReadOnlyList<LedKey> hotkeys, IHotkeyCommand command)
        {
            if (hotkeys.Count < 1)
                return;

            tree.CreateNode(hotkeys, command);
        }
        public bool DeleteHotKey(List<LedKey> hotkeys)
        {
            if (hotkeys.Count < 1)
                return false;


            return !tree.RemoveNode(CollectionsMarshal.AsSpan(hotkeys));
        }


        /// <summary>
        /// Records all <see cref="LedKey"/> pressed till <see cref="VulcanKeyboard.VolumeKnobTurnedReceived"/> is received
        /// </summary>
        public async Task<ReadOnlyCollection<LedKey>> RecordKeys(CancellationToken token)
        {
            synchronRecordingKeys.Clear();
            ledKeys.Clear();
            AddHotKeyEffect();
            isSynchronRecording = true;
            StartedHotkeyMode?.Invoke(this, new());
            try
            {
                await synchronRecordingSemaphore.WaitAsync(token);
            }
            catch (OperationCanceledException x)
            {
            }
            StoppedHotkeyMode?.Invoke(this, new());

            isSynchronRecording = false;
            RemoveHotKeyEffect();

            return new ReadOnlyCollection<LedKey>(synchronRecordingKeys);
        }

        /// <summary>
        /// Records all <see cref="LedKey"/> pressed till <see cref="VulcanKeyboard.VolumeKnobTurnedReceived"/> is received
        /// </summary>
        public ReadOnlyCollection<(LedKey, int)> RecordKeysWithTime()
        {
            var recWithTime = new List<(LedKey, int)>();
            Stopwatch stopWatch = new Stopwatch();

            keyboard.KeyPressedReceived += (s, e) =>
            {
                if (e.IsPressed)
                {
                    recWithTime.Add((e.Key, (int)stopWatch.Elapsed.TotalMilliseconds));
                    stopWatch.Restart();
                }
            };
            synchronRecordingKeys.Clear();
            ledKeys.Clear();
            AddHotKeyEffect();
            isSynchronRecording = true;

            synchronRecordingSemaphore.Release();
            isSynchronRecording = false;
            RemoveHotKeyEffect();

            return new ReadOnlyCollection<(LedKey, int)>(recWithTime);
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

            ledKeys.Clear();
            AddHotKeyEffect();
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
                RemoveHotKeyEffect();
            }
        }

        public IEnumerable<(List<LedKey> keys, IHotkeyCommand command)> GetHotkeys() => tree.GetCommands();


        private void Keyboard_KeyPressedReceived(object? sender, KeyPressedArgs e)
        {
            if (isSynchronRecording || isAsyncRecording)
            {
                if (e.IsPressed)
                {
                    ledKeys.Clear();

                    if (isSynchronRecording)
                    {
                        if (synchronRecordingKeys.Count > 0)
                            ledKeys.Add(synchronRecordingKeys[^1]);

                        synchronRecordingKeys.Add(e.Key);
                    }
                    else if (isAsyncRecording)
                    {
                        if (lastAsyncRecordingKey is not null)
                            ledKeys.Add(lastAsyncRecordingKey.Value);

                        lastAsyncRecordingKey = e.Key;
                        asyncTaskCompletionSource?.SetResult(e.Key);
                    }

                    ledKeys.Add(e.Key);
                }

                return;
            }

            var lastNode = currentNode;
            if (e.Key == HotKey && Mode == HotKeyMode.Active)
            {
                IsExecuting = e.IsPressed;
                if (!e.IsPressed)
                    lastNode.Command?.Execute();

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
                    lastNode.Command?.Execute();
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
                Task.Run(() => node.SinglePathChild!.Command?.Execute());
                IsExecuting = false;
                return true;
            }

            return false;
        }

        private void SetHotKeysColoring()
        {
            ledKeys.Clear();
            exitKeys.Clear();
            if (currentNode is not null)
            {
                foreach (var item in currentNode.Children)
                    ledKeys.Add(item.Key);

                if (Mode == HotKeyMode.Passive)
                    exitKeys.Add(ExitKey);

                if (currentNode.Command is not null)
                {
                    switch (Mode)
                    {
                        case HotKeyMode.Passive:
                            exitKeys.Add(EarlyExitKey);
                            break;
                        case HotKeyMode.Active:
                            exitKeys.Add(HotKey);
                            break;
                    }
                }
            }

        }

        private void RemoveHotKeyEffect()
        {
            lightService.RemoveOverrideEffect(solidBlackEffect);
            lightService.RemoveOverrideEffect(hotKeyEffect);
            lightService.RemoveOverrideEffect(earlyExitEffect);
        }
        private void AddHotKeyEffect()
        {
            lightService.AddOverrideToEnd(solidBlackEffect);
            lightService.AddOverrideToEnd(hotKeyEffect);
            lightService.AddOverrideToEnd(earlyExitEffect);
        }


    }
}
