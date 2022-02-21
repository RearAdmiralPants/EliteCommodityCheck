namespace EliteCommodityAnalysis.Abstractions.ApiParameters {
    
    using System.Collections.Generic;
    /* Kind of a hacky way to get around generic type-based dependency injection for each Potential API that the app will connect to. */
    public class EddbParameters : IApiParameters {
        private Dictionary<string, string> Parameters {get;} = new Dictionary<string, string>();

        public EddbParameters() {
            this.Parameters.Add("ApiName", "EDDB");
            // How many minutes the tool should wait before checking EDDB each time
            this.Parameters.Add("MinutesPastRequiringNewDownload", "30");
            /// A value representing how many minutes must elapse after header retrieval until they must be retrieved again
            this.Parameters.Add("MinutesPastRequiringNewHeaderRefresh", "720");
        }

        public string GetParameter(string paramName) {
            return this.Parameters[paramName];
        }
    }
}