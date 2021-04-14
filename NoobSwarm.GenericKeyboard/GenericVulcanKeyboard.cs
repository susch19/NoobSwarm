using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NoobSwarm.GenericKeyboard.Linux;
using Vulcan.NET;
using NoobSwarm.Makros;
using Key = NoobSwarm.Makros.Key;

namespace NoobSwarm.GenericKeyboard
{
    public class GenericVulcanKeyboard : IVulcanKeyboard
    {
        
        public byte Brightness { get; set; }

        public event EventHandler<VolumeKnDirectionArgs> DPITurnedReceived;
        public event EventHandler<KeyPressedArgs> KeyPressedReceived;
        public event EventHandler<TestArgs> TestKeyPressedReceived;
        public event EventHandler<VolumeKnobFxArgs> VolumeKnobFxPressedReceived;
        public event EventHandler<VolumeKnobArgs> VolumeKnobPressedReceived;
        public event EventHandler<VolumeKnDirectionArgs> VolumeKnobTurnedReceived;

        public void InvokeDPITurnedReceived(VolumeKnDirectionArgs args) => DPITurnedReceived?.Invoke(this, args);
        public void InvokeKeyPressedReceived(KeyPressedArgs args) => KeyPressedReceived?.Invoke(this, args);
        public void InvokeTestKeyPressedReceived(TestArgs args) => TestKeyPressedReceived?.Invoke(this, args);
        public void InvokeVolumeKnobFxPressedReceivedd(VolumeKnobFxArgs args) => VolumeKnobFxPressedReceived?.Invoke(this, args);
        public void InvokeVolumeKnobPressedReceived(VolumeKnobArgs args) => VolumeKnobPressedReceived?.Invoke(this, args);
        public void InvokeVolumeKnobTurnedReceived(VolumeKnDirectionArgs args) => VolumeKnobTurnedReceived?.Invoke(this, args);

        public Key VolumeKnobTurnedKey { get; } = Key.NUMLOCK;
        public Key FnKey { get; } = Key.CAPITAL;//Makros.Key.APPS;

        private KeyboardHook hook;

        private static KeyboardHook CreatePlatformKeyboardHook()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new LowLevelKeyboardHookLinux();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new LowLevelKeyboardHookWindows();

            throw new NotSupportedException("Operating system not supported!");
        }
        public GenericVulcanKeyboard()
            : this(CreatePlatformKeyboardHook())
        {
            
        }

        public GenericVulcanKeyboard(KeyboardHook hook)
        {
            this.hook = hook;
            hook.OnKeyPressed += Hook_OnKeyPressed;
            hook.OnKeyUnpressed += Hook_OnKeyUnpressed;
            hook.HookKeyboard(FnKey);
        }

        private void Hook_OnKeyUnpressed(object sender, Key e)
        {
            if (e == VolumeKnobTurnedKey)
                return;
            if(e == FnKey)
            {
                KeyPressedReceived?.Invoke(this, new KeyPressedArgs(LedKey.FN_Key, false));
                return;
            }

            if (LedKeyToKeyMapper.KeyToLedKey.TryGetValue(e, out var ledKey))
            {
                KeyPressedReceived?.Invoke(this, new KeyPressedArgs(ledKey, false));
            }
        }

        private void Hook_OnKeyPressed(object sender, Key e)
        {
            if (e == VolumeKnobTurnedKey)
            {
                VolumeKnobTurnedReceived?.Invoke(this, new VolumeKnDirectionArgs(0, true));
                return;
            }
            if (e == FnKey)
            {
                KeyPressedReceived?.Invoke(this, new KeyPressedArgs(LedKey.FN_Key, true));
                return;
            }

            if (LedKeyToKeyMapper.KeyToLedKey.TryGetValue(e, out var ledKey))
            {
                KeyPressedReceived?.Invoke(this, new KeyPressedArgs(ledKey, true));
            }
        }

        public bool Connect()
        => true;

        public void Disconnect()
        {
        }

        public byte[] GetLastSendColorsCopy()
            => Array.Empty<byte>();

        public void SetColor(Color clr)
        {
        }

        public void SetColors(Dictionary<int, Color> keyColors)
        {
        }

        public void SetColors(Dictionary<LedKey, Color> keyColors)
        {
        }

        public bool SetColors(ReadOnlySpan<byte> keyColors)
        {
            return true;
        }

        public void SetKeyColor(int key, Color clr)
        {
        }

        public void SetKeyColor(LedKey key, Color clr)
        {
        }

        public async Task<bool> Update()
        {
            return true;
        }
    }
}
