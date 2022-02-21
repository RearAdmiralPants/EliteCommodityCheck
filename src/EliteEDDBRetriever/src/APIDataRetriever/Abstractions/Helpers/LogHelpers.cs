namespace EliteCommodityAnalysis.Abstractions.Helpers {
    using SimpleLogger.Managers;
    using SimpleLogger.Abstractions;
    using SimpleLogger.Abstractions.Logging;
    using LoggingHelpers = SimpleLogger.Abstractions.Helpers;

    using System.Threading.Tasks;

    public class LogFileHelpers {
        private IDebugService DebugService;

        private IDebugMessage DebugMessage;

        public LogFileHelpers(IDebugService service, IDebugMessage msg) {
            this.DebugService = service;
            this.DebugMessage = msg;
        }

        public string LogDirecotry { get; set; } = PerHostAppConstants.DEFAULT_LOG_DIRECTORY;

        public string LogFilename { get; set; }

        public string GetTimestampFilename() { 
            return LoggingHelpers.StringHelpers.UtcNowToFilesafeString() + ".log";
        }

        public async Task LogStringToFileAsync(string contents, bool append = true) {
            var msg = this.DebugMessage;
            msg.Reset();
            msg.Contents = contents;
            
            var output = new FileOutput(msg);
            output.LogDirectory = this.LogDirecotry;
            output.Filename = this.LogFilename;
            output.Append = append;
            
            msg.Outputs.Add(output);

            await this.DebugService.ProcessMessageAsync(msg);
        }

        public void LogStringToFile(string contents, bool append = true) {
            var msg = this.DebugMessage;
            msg.Reset();
            msg.Contents = contents;
            
            var output = new FileOutput(msg);
            output.LogDirectory = this.LogDirecotry;
            output.Filename = this.LogFilename;
            output.Append = append;
            
            msg.Outputs.Add(output);

            this.DebugService.ProcessMessageAsync(msg);
        }

        public async Task LogStringToConsoleAsync(string contents) {
           var msg = this.DebugMessage;
           msg.Reset();
           
           msg.Contents = contents;

           var output = new ConsoleOutput(msg);
           await output.WriteOutputAsync();
        }

        public void LogStringToConsole(string contents) {
           var msg = this.DebugMessage;
           msg.Reset();
           
           msg.Contents = contents;

           var output = new ConsoleOutput(msg);
           output.WriteOutput();
        }
    }
}