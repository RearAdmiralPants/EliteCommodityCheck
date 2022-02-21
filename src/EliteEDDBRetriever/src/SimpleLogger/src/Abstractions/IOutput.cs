namespace SimpleLogger.Abstractions {
    using System.Threading.Tasks;

    public interface IOutput {

        void WriteOutput();

        Task WriteOutputAsync();

        IDebugMessage Parent {get; set;}
    }
}