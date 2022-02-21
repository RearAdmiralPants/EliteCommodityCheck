namespace SimpleLogger.Abstractions.Logging {
    using System;
    using System.Threading.Tasks;

    // A REALLY convoluted way to do Console.WriteLine(). Mostly a proof-of-concept of .NET Core's dependency injection framework.
    public class ConsoleOutput : IOutput {

        public ConsoleOutput(IDebugMessage parent) {
            this.Parent = parent;
        }

        public IDebugMessage Parent {get; set;}

        public void WriteOutput() {
            Console.WriteLine(this.Parent.GetOutput().Trim());
        }

        public async Task WriteOutputAsync() {
            await Task.Run(() => { Console.WriteLine(this.Parent.GetOutput().Trim()); });
        }

    }
}