namespace SimpleLogger.Abstractions.Logging {
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public class FileOutput : IOutput {
        public FileOutput() { }
        public FileOutput(IDebugMessage parent) {
            this.Parent = parent;
        }

        ////TODO: Add some constructors here to obviate the need for APIDataRetriever.Abstractions.Helpers.LogHelpers

        public IDebugMessage Parent {get; set;}

        public string LogDirectory {get; set; } = null;
        public string Filename { get; set; } = Helpers.StringHelpers.UtcNowToFilesafeString() + ".log";

        public string AbsoluteFilename { 
            get {
                if (string.IsNullOrEmpty(this.LogDirectory)) { throw new System.ApplicationException("No log directory supplied."); }
                return Path.Combine(this.LogDirectory, this.Filename);
            }
        }

        public bool Append {get; set; } = true;
        public int MaxLogfileSize {get; set; } = 10000000;

        public void WriteOutput() {
            this.MaintainLogFile();
            
            var builder = new StringBuilder();
            builder.Append(this.Parent.GetOutput().Trim());
            File.AppendAllText(this.AbsoluteFilename, builder.ToString());
        }

        public async Task WriteOutputAsync() {
            this.MaintainLogFile();

            var builder = new StringBuilder();
            builder.Append(this.Parent.GetOutput().Trim());
            await File.AppendAllTextAsync(this.AbsoluteFilename, builder.ToString());
        }

        private void MaintainLogFile() {
            if (this.Parent == null) {
                throw new ApplicationException("IOutput.Parent is a required property and cannot remain NULL when logging begins execution.");
            }

            if (!Directory.Exists(this.LogDirectory)) {
                Directory.CreateDirectory(this.LogDirectory);
            }
            if (File.Exists(this.AbsoluteFilename)) {
                var info = new FileInfo(this.AbsoluteFilename);
                if (info.Length >= this.MaxLogfileSize) {
                    this.Append = false;
                }
            }
            if (!this.Append) {
                var fileStream = File.Create(this.AbsoluteFilename);
                fileStream.Close();
                fileStream.Dispose();
            }
        }
    }
}