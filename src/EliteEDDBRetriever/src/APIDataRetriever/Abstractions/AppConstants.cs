namespace EliteCommodityAnalysis.Abstractions {

    ////TODO: create sublcasses per constant type
    public static class AppConstants {
        // How many minutes the tool should wait before checking EDDB each time
        public const int MINUTES_PAST_REQUIRING_NEW_DOWNLOAD = 30;

        /// A value representing how many minutes must elapse after header retrieval until they must be retrieved again
        public const int MINUTES_PAST_REQUIRING_NEW_HEADER_REFRESH = 720;

        /// A value representing whether to echo local debug message to the console
        public const bool ECHO_DEBUG_MDESSAGES = true;

        /// A value indicating the character to use to separate directories. Hopefully forward slashes work on Windows adequately in core 3.1.x
        public const string DIRECTORY_SEPARATOR = "/";
    }

}