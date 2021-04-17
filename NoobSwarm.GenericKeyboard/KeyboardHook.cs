using System;
using NoobSwarm.Makros;

namespace NoobSwarm.GenericKeyboard
{
    public abstract class KeyboardHook : IDisposable
    {
        public event EventHandler<Key> OnKeyPressed;
        public event EventHandler<Key> OnKeyUnpressed;

        protected void RaiseOnKeyPressed(Key key) => OnKeyPressed?.Invoke(this, key);
        protected void RaiseOnKeyUnpressed(Key key) => OnKeyUnpressed?.Invoke(this, key);

        public abstract void HookKeyboard(Key startKey);
        
        public abstract void Dispose();

        public abstract void SetSupressKeyPress(bool value);
      
    }
}