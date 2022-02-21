namespace SimpleLogger.Managers {
    using System;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Logging;
 
    public class DebugService : IDebugService {

        private IDebugMessage DebugMessage;

        public DebugService(IDebugMessage debugMessage) {
            this.DebugMessage = debugMessage;
        }

        public void ProcessMessage(IDebugMessage message)
        {
            if (message.Processed) {
                throw new InvalidOperationException("This log message has already been processed.");
            }

            foreach (var output in message.Outputs) {
                output.WriteOutput();
            }

            message.Processed = true;
        }

        public async Task ProcessMessageAsync(IDebugMessage message) {
            if (message.Processed) {
                throw new InvalidOperationException("This log message has already been processed.");
            }

            foreach (var output in message.Outputs) {
                await output.WriteOutputAsync();
            }

            message.Processed = true;
        }

        public void LogString(string message, params LogOutputs[] outputs) {
            this.DebugMessage.Reset(message);
            foreach (var output in outputs) {
                if (output == LogOutputs.Console) { this.DebugMessage.Outputs.Add((IOutput)new ConsoleOutput(this.DebugMessage)); }
                if (output == LogOutputs.File) { this.DebugMessage.Outputs.Add((IOutput)new FileOutput(this.DebugMessage)); }
            }
            this.ProcessMessage(this.DebugMessage);
        }

        public async Task LogStringAsync(string message, params LogOutputs[] outputs) {
            this.DebugMessage.Reset(message);
            foreach (var output in outputs) {
                if (output == LogOutputs.Console) { this.DebugMessage.Outputs.Add((IOutput)new ConsoleOutput(this.DebugMessage)); }
                if (output == LogOutputs.File) { this.DebugMessage.Outputs.Add((IOutput)new FileOutput(this.DebugMessage)); }
            }
            await this.ProcessMessageAsync(this.DebugMessage);
        }

    }
}