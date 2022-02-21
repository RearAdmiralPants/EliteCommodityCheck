namespace EliteCommodityAnalysis.Abstractions {
    using System;
    
    public class ConsoleKeyData : IConsoleKeyData {
        public ConsoleKeyData() { }

        public ConsoleKeyData(ConsoleKey pressedKey) {
            this.KeyPressed = pressedKey;
        }

        public ConsoleKey KeyPressed { get; set; }

        public DateTime KeyPressedTimestamp { get; set; } = DateTime.UtcNow;
    }
}