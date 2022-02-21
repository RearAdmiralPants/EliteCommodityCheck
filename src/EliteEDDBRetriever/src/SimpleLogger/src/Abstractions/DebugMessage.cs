namespace SimpleLogger.Abstractions
{
    using System;
    using System.Collections.Generic;

    public class DebugMessage : IDebugMessage
    {
        public DebugMessage() { }

        public DebugMessage(string contents) {
            this.Contents = contents;
        }

        public DebugMessagePrefixType PrefixType { get; set; } = DebugMessagePrefixType.String;

        public string Prefix { get; set; }

        private string _contents;
        public string Contents { 
            get { 
                return _contents;
            }
            set {
                _contents = value;
                this._created = DateTime.UtcNow;
            }
        }

        private DateTime _created = DateTime.UtcNow;
        public DateTime Created {
            get {
                return this._created;
            }
        }

        public bool Processed {get; set;} = false;

        public List<IOutput> Outputs { get; private set; } = new List<IOutput>();

        public string GetOutput() {
            if (this.PrefixType == DebugMessagePrefixType.String) {
                return this.Prefix + " " + this.Contents;
            }
            else if (this.PrefixType == DebugMessagePrefixType.Timestamp) {
                return this.Created.ToString() + " " + this.Contents;
            }
            else {
                // Should never occur
                return this.Contents;
            }
        }

        public void Reset(string newContents) {
            this.Prefix = null;
            this.Contents = newContents;
            this.Processed = false;
            this._created = DateTime.UtcNow;
            this.Outputs.Clear();

        }

        public void Reset() {
            this.Reset(null);
        }
    }
}
