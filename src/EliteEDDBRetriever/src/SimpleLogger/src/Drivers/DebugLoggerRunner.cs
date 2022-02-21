namespace SimpleLogger.Drivers {
    using System;
    using System.Threading.Tasks;
    using System.IO;

    using Abstractions;
    using Abstractions.Logging;
    using Managers;

    using System.Collections.Generic;
    public class DebugLoggerRunner {

        private readonly IDebugService DebugService;
        private LogOutputs[] ConsoleOnly;
        private LogOutputs[] FileOnly;
        private LogOutputs[] BothOutputs;

        public DebugLoggerRunner(IDebugService service) {
            this.DebugService = service;
            var list = new List<LogOutputs>();
            list.Add(LogOutputs.Console);
            this.ConsoleOnly = list.ToArray();

            list.Clear();
            list.Add(LogOutputs.File);
            this.FileOnly = list.ToArray();

            list.Clear();
            list.Add(LogOutputs.File);
            list.Add(LogOutputs.Console);
            this.BothOutputs = list.ToArray();

            list.Clear();
            list = null;
        }
        public async Task Run() {
            Console.WriteLine("Beginning exercise...");
            //Task.WaitAll(SubmitLogMessagesUntilKeypress());
            await SubmitLogMessagesUntilKeypress();
            //Task.Run(() => SubmitLogMessagesUntilKeypress());
        }

        public async Task SubmitLogMessagesUntilKeypress() {
            ConsoleKeyInfo keyInfo;
            
            do {
            keyInfo = Console.ReadKey();

            await this.DebugService.LogStringAsync("User pressed keycode " + keyInfo.Key.ToString() + "!", this.ConsoleOnly);

            } while (keyInfo.Key.ToString().ToUpperInvariant() != "Q");

            await this.DebugService.LogStringAsync("User wishes to quit!", this.ConsoleOnly);

            await this.DebugService.LogStringAsync("Press a key to exercise dual logging...", this.ConsoleOnly);
            Console.ReadKey();

            await this.DebugService.LogStringAsync("Type a line to exercise all-output debugging.", this.ConsoleOnly);
            var lineMsg = Console.ReadLine();

            await this.DebugService.LogStringAsync(lineMsg, this.BothOutputs);
            await this.DebugService.LogStringAsync("Finished!", this.ConsoleOnly);

            await this.DebugService.LogStringAsync("Press a key to exercise file logging...", this.ConsoleOnly);
            Console.ReadKey();

            var msgBuilder = new System.Text.StringBuilder();
            for (int buildString = 0; buildString < 100000; buildString++) {
                msgBuilder.Append("Paul is learning about stuff that is fun but tedious and whatnot !!!!!!!\t");
            }
            var logMsg = msgBuilder.ToString();

            await this.DebugService.LogStringAsync("Logging message of length " + logMsg.Length.ToString() + " to file...", this.ConsoleOnly);
            DateTime start = DateTime.UtcNow;
            await this.DebugService.LogStringAsync(logMsg, this.FileOnly);
            DateTime stop = DateTime.UtcNow;

            var runtime = stop.Subtract(start);

            await this.DebugService.LogStringAsync("Complete in " + runtime.TotalSeconds.ToString("F2") + " seconds!", this.ConsoleOnly);
        }
    }
}