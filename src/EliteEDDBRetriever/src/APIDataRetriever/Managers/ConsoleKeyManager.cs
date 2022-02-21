namespace EliteCommodityAnalysis.Managers {
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Timers;

    using Abstractions;
    public class ConsoleKeyManager : IConsoleKeyManager {
        
        private Timer KeyCheckTimer;

        private static object myLocker = new object();

        public ConsoleKeyManager(IConsoleKeysPressed keys) {
            this.KeysPressed = keys;
        }

        public IConsoleKeysPressed KeysPressed { get; private set; }
        public void StartReading() {
            // Using Timers is OK here, since there's no asynchronous behavior
            this.KeyCheckTimer = new Timer();
            this.KeyCheckTimer.Interval = 1000;
            this.KeyCheckTimer.Elapsed += this.KeyCheckTimerElapsed;
            this.KeyCheckTimer.AutoReset = false;
            this.KeyCheckTimer.Start();
        }

        public void StopReading() {
            this.KeyCheckTimer.Elapsed -= this.KeyCheckTimerElapsed;
            this.KeyCheckTimer.Stop();
        }

        public void KeyCheckTimerElapsed(object sender, EventArgs eventArgs) {
            if (Console.KeyAvailable)
            {
                var data = new ConsoleKeyData(Console.ReadKey(true).Key);
                this.KeysPressed.KeysPressed.Add((IConsoleKeyData)data);
            }
            this.KeyCheckTimer.Start();
        }

        public IConsoleKeyData HasKey(ConsoleKey key, DateTime? minTimestamp = null) {
            IConsoleKeyData result = null;
            if (minTimestamp != null) {
                result = this.KeysPressed.KeysPressed.FirstOrDefault(k => k.KeyPressed == key && k.KeyPressedTimestamp >= minTimestamp);
            }
            else {
                result = this.KeysPressed.KeysPressed.FirstOrDefault(k => k.KeyPressed == key);
            }

            return result;
        }

        public void RemoveKey(IConsoleKeyData toRemove) {
            lock (myLocker) {
                this.KeysPressed.KeysPressed = new ConcurrentBag<IConsoleKeyData>(this.KeysPressed.KeysPressed.Except(new[] { toRemove }));
            }
        }

        public void RemoveKeys(ConsoleKey key) {
            var toRemove = this.KeysPressed.KeysPressed.Where(k => k.KeyPressed == key).ToArray();
            lock (myLocker) {
                this.KeysPressed.KeysPressed = new ConcurrentBag<IConsoleKeyData>(this.KeysPressed.KeysPressed.Except(toRemove));
            }
        }

        public void RemoveAllKeys() {
            this.KeysPressed.KeysPressed.Clear();
        }

        public void Dispose() {
            if (this.KeyCheckTimer != null) {
                if (this.KeyCheckTimer.Enabled) {
                    this.KeyCheckTimer.Stop();
                }
                this.KeyCheckTimer.Dispose();
            }
            this.KeysPressed.Dispose();
        } 
    }
}