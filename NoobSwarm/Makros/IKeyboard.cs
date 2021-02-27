using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NoobSwarm.Makros
{
    public interface IKeyboard
    {
        Task PlayMacro(IReadOnlyList<MakroManager.RecordKey> recKeys);
        void SendChar(char charToTest);
        void SendChar(char charToTest, KeyModifier modifier);
        bool SendCharsSequene(ReadOnlySpan<char> charsToTest);
        void SendVirtualKey(ushort key);
        void SendVirtualKey(ushort key, KeyModifier modifier);
        void SendVirtualKeysSequence(ReadOnlySpan<ushort> keys, KeyModifier modifier);
    }
}