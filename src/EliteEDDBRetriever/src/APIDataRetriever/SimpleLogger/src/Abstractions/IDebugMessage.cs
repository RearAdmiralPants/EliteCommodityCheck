namespace SimpleLogger.Abstractions {
    using System;
    using System.Collections.Generic;

    ////TODO: Get rid of this; no need for interfaces for model-only classes (no methods to unit test)
    public interface IDebugMessage {
        DebugMessagePrefixType PrefixType { get; set; }

        string Prefix { get; set; }

        string Contents {get; set; }

        DateTime Created {get; }

        bool Processed { get; set; }

        List<IOutput> Outputs { get; }

        string GetOutput();

        void Reset();

        void Reset(string newContents);
    }
}