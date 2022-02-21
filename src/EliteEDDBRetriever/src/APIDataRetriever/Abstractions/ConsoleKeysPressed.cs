namespace EliteCommodityAnalysis.Abstractions {
    using System.Collections.Concurrent;

    public class ConsoleKeysPressed : IConsoleKeysPressed {

        public ConcurrentBag<IConsoleKeyData> KeysPressed { get; set; }

        public void Dispose() {
            this.KeysPressed.Clear();
            this.KeysPressed = null;
        }

   }
}