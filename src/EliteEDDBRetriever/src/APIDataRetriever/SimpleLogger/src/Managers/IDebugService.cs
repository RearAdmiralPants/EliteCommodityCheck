namespace SimpleLogger.Managers {
    using Abstractions;
    using Abstractions.Logging;
    using System.Threading.Tasks;
    public interface IDebugService {
        void ProcessMessage(IDebugMessage message);

        Task ProcessMessageAsync(IDebugMessage message);

        void LogString(string message, params LogOutputs[] outputs);

        Task LogStringAsync(string message, params LogOutputs[] outputs);
    }
}