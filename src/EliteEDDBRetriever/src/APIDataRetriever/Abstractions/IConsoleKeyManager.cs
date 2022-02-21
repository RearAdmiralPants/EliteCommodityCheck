namespace EliteCommodityAnalysis.Abstractions {
    using System;
    
    public interface IConsoleKeyManager : IDisposable {
        IConsoleKeysPressed KeysPressed { get; }

        void StartReading();

        void StopReading();

        IConsoleKeyData HasKey(ConsoleKey key, DateTime? minTimestamp);

        void RemoveKey(IConsoleKeyData toRemove);

        void RemoveKeys(ConsoleKey key);

        void RemoveAllKeys();
    }
}