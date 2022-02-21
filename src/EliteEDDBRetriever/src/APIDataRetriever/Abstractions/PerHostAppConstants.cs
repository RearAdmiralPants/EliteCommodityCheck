namespace EliteCommodityAnalysis.Abstractions {
    /* This implementation should go away in favor of a user-configurable file from one of many prebuilt frameworks e.g. Microsoft.Configuration */
    public static class PerHostAppConstants
    {
        public const string DEFAULT_ROOT_DIRECTORY = @"C:\src\EDDBData\";

        public const bool LOG_DEBUG_MESSAGES = true;

        public const bool ECHO_DEBUG_MESSAGES = true;

        public const string DEFAULT_LOG_DIRECTORY = @"C:\src\EDDBData\Logs\";

        public const bool GET_UNCHANGED_FILES = true;

        public const string SYSTEMS_EDDB_JSON_FILE_PATH = @"C:\src\EDDBData\20210307\systems_populated.json";

    }
}