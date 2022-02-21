namespace EliteCommodityAnalysis.Abstractions.Helpers {
    
    using SimpleLogger.Abstractions.Logging;
    public static class LogOutputHelpers {
        public static LogOutputs[] ConsoleOnly() {
            return new LogOutputs[] { LogOutputs.Console };
        }

        public static LogOutputs[] FileOnly()
        {
            return new LogOutputs[] { LogOutputs.File };
        }

        public static LogOutputs[] ConsoleAndFile()
        {
            return new LogOutputs[] { LogOutputs.Console, LogOutputs.File };
        }
    }
}