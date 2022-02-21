namespace EliteCommodityAnalysis.Abstractions
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using SimpleLogger.Managers;

    using Helpers;

    //// TODO: This should be the only record and source of data for API modification times and file sizes, and possibly local ones as well.
    //// TODO: Refactor with HttpClient and classes

    /* As such, the DI paradigm here would be global or one instance per application. */
    public class HttpHeaderReceiver : HttpClient
    {
        private const string LAST_MODIFIED = "Last-Modified";
        private const string CONTENT_LENGTH = "Content-Length";
        private const string HEADERS_TARGET = "HeadersTarget";
        private const string HEADERS_REFRESHED = "HeadersRefreshed";
        
        //// TODO: Combine HttpResponseHeaders, DateTime, long, etc. into single class
        private Dictionary<string, HttpContentHeaders> httpHeaders = null;
        private Dictionary<string, DateTimeOffset?> httpHeadersRetrieved = null;
        private Dictionary<string, long> httpFileSize = null;
        private Dictionary<string, DateTimeOffset> httpHeadersAttempted = null;
        private bool isRefreshing = false;

        private Dictionary<string, string> appHeaders = null;
        private DateTime lastRefreshed = DateTime.MinValue;
        private List<Dictionary<string, string>> AllUriAppHeaders = null;
        private object locked = "locker";
        private IDebugService DebugService;

        private string currentUri = null;

        public HttpHeaderReceiver(IDebugService debugService) : base() {
            this.DebugService = debugService;
            if (this.AllUriAppHeaders == null) { this.AllUriAppHeaders = new List<Dictionary<string, string>>(); }
            this.httpHeaders = new Dictionary<string, HttpContentHeaders>();
            this.httpHeadersRetrieved = new Dictionary<string, DateTimeOffset?>();
            this.httpFileSize = new Dictionary<string, long>();
            this.httpHeadersAttempted = new Dictionary<string, DateTimeOffset>();
        }

        public DateTime LastGlobalRefresh { get; private set; } = DateTime.MinValue;

        public async Task RefreshHeadersIfNecessary(Uri reqUri)
        {
            /*
            var result = this.httpHeadersRetrieved[this.UriToString(reqUri)];
            if (this.isRefreshing)
            {
                if (result == DateTimeOffset.MinValue)
                {
                    result = this.httpHeadersAttempted[this.UriToString(reqUri)];
                }
            }
            */

            var lastModified = DateTimeOffset.MinValue;
            var lastAttempted = DateTimeOffset.MinValue;

            if (this.httpHeadersRetrieved.ContainsKey(this.UriToString(reqUri)))
            {
                lastModified = this.httpHeadersRetrieved[UriToString(reqUri)].Value;
            }
            //lastModified = await this.GetLastModifiedHeaderTimeAsync(reqUri);
            if (this.httpHeadersAttempted.ContainsKey(UriToString(reqUri)))
            {
                lastAttempted = this.httpHeadersAttempted[UriToString(reqUri)];
            }

            var uriString = this.UriToString(reqUri);
            var sinceLastRetrieval = DateTimeOffset.UtcNow.Subtract(lastAttempted).TotalMinutes;
            if (sinceLastRetrieval >= AppConstants.MINUTES_PAST_REQUIRING_NEW_HEADER_REFRESH)
            {
                if (!isRefreshing)
                {
                    try
                    {
                        this.isRefreshing = true;
                        await DebugService.LogStringAsync("Retrieving headers; did so " + sinceLastRetrieval.ToString() + " minutes ago...", LogOutputHelpers.ConsoleOnly());

                        var msg = new HttpRequestMessage(HttpMethod.Head, uriString);
                        var response = await new HttpClient().SendAsync(msg);
                        this.httpHeaders[reqUri.ToString()] = response.Content.Headers;
                        this.PopulateHeadersCollections(uriString);
                        this.httpHeadersAttempted[uriString] = DateTimeOffset.UtcNow;
                        this.isRefreshing = false;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Backwards compatibility
        /// TODO: Refactor/delete
        /// </summary>
        /// <param name="reqUri"></param>
        /// <returns></returns>
        public async Task<ulong> GetFilesize(Uri reqUri)
        {
            await this.RefreshHeadersIfNecessary(reqUri);
            if (this.httpFileSize.ContainsKey(this.UriToString(reqUri))) {
                return Convert.ToUInt64(System.Math.Abs(this.httpFileSize[this.UriToString(reqUri)]));
            }
            return 0;
        }

        public async Task<DateTime> GetLastModified(Uri reqUri)
        {
            var result = await this.GetLastModifiedHeaderTimeAsync(reqUri);
            return result.DateTime;
        }



        private void PopulateHeadersCollections(string url)
        {
            if (this.httpHeaders[url].LastModified is not null)
            {
                this.httpHeadersRetrieved[url] = this.httpHeaders[url].LastModified;
            }

            if (this.httpHeaders[url].ContentLength is not null)
            {
                var headerValue = this.httpFileSize[url] = this.httpHeaders[url].ContentLength.Value;
            }

        }

        /*
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req)
        {
            this.currentUri = req.RequestUri.ToString();
            req.Method = HttpMethod.Head;

            return await base.SendAsync(req).ConfigureAwait(false);

            //return base.GetResponse(request);
        }
        */
        protected async Task<HttpResponseMessage> RetrieveHeadersAsync(HttpRequestMessage req)
        {
            this.currentUri = req.RequestUri.ToString();
            req.Method = HttpMethod.Head;

            var client = new HttpClient();

            var response = await client.SendAsync(req);

            return response;
        }


        /*
    private async Task PopulateHeadersCollection(Uri reqUri)
    {
        var msg = new HttpRequestMessage();
        msg.RequestUri = reqUri;

        var res = await this.RetrieveHeadersAsync(msg);
        this.headers = res.Headers;
        this.lastRefreshed = DateTime.UtcNow;

        await this.PopulateAppHeaders();
        await this.UpdateHeadersCollection(reqUri.ToString());


    }
        

        private async Task UpdateHeadersCollection(string reqUri) {
            // foreach (var uri in new List<string>(this.appHeaders.Keys)) {

            // }
            var theIndex = this.AllUriAppHeaders.FirstOrDefault(entry => entry[HEADERS_TARGET] == reqUri);
                if (theIndex != null) {
                var updateIndex = this.AllUriAppHeaders.IndexOf(theIndex);
                    if (updateIndex == -1) {
                        await this.DebugService.LogStringAsync("**************** Got Index -1 updating AllUriAppHeaders", LogOutputHelpers.ConsoleOnly());
                    }
                    else {
                        this.AllUriAppHeaders[updateIndex] = new Dictionary<string, string>(this.appHeaders);
                    }
                }
                else {
                    this.AllUriAppHeaders.Add(new Dictionary<string, string>(this.appHeaders));
                }
            
            //this.headers = null;
            // This was/should be commented out because now we are dealing with one lastRefreshed per file/URI
            // this.lastRefreshed = DateTime.MinValue;
                
        }

        private bool AppHeadersExist(string reqUri) {
            var theIndex = this.AllUriAppHeaders.FirstOrDefault(entry => entry?[HEADERS_TARGET] == reqUri);
                if (theIndex != null) {
                var updateIndex = this.AllUriAppHeaders.IndexOf(theIndex);
                if (updateIndex > -1) {
                    return true;
                }
                }
                return false;
        }

        private void GetAppHeaders(string reqUri) {
            var theIndex = this.AllUriAppHeaders.FirstOrDefault(entry => entry?[HEADERS_TARGET] == reqUri);
                if (theIndex != null) {
                var updateIndex = this.AllUriAppHeaders.IndexOf(theIndex);
                    this.appHeaders = this.AllUriAppHeaders[updateIndex];
                }
                else {
                    this.appHeaders = new Dictionary<string, string>();
                }
        }

        private async Task PopulateAppHeaders() {

            /*
            var headers = this.headers;
            // this.appHeaders = new Dictionary<string, string>();
            this.GetAppHeaders(this.currentUri);
            //lock(locked) {
                this.appHeaders[HEADERS_TARGET] = this.currentUri;
                this.appHeaders[HEADERS_REFRESHED] = DateTime.UtcNow.ToString();
                
                foreach (string key in headers.Keys) {
                    if (this.appHeaders.ContainsKey(key) && this.appHeaders[key] == headers[key]) {}
                    else {
                    var appValue = this.headers[key].ToString();
                    if (key == LAST_MODIFIED) {
                        try {
                            var nonNumeric = appValue.ToCharArray().FirstOrDefault(c => !char.IsNumber(c));
                                if (string.IsNullOrWhiteSpace(nonNumeric.ToString())) {
                                var dt = new DateTime(long.Parse(appValue), DateTimeKind.Utc);
                                    appValue = dt.ToString();
                                }
                         }
                         catch(Exception) {
                             var directDt = DateTime.Parse(appValue, null, System.Globalization.DateTimeStyles.AssumeUniversal);
                             appValue = directDt.ToString();
                         }
                    }
                    this.appHeaders[key] = appValue;
                    }
                }

                await this.UpdateHeadersCollection(this.currentUri);

                //this.AllUriAppHeaders.Add(this.appHeaders);
                //this.PopulateHeadersCollection(this.appHeaders[HEADERS_TARGET]);
            //}
            

            var headers = this.httpHeaders;

        }

        /*
        private bool HeadersRequireRefresh(string reqUri)
        {
            if (this.AppHeadersExist(reqUri) || DateTime.UtcNow.Subtract(this.lastRefreshed).TotalMinutes > AppConstants.MINUTES_PAST_REQUIRING_NEW_HEADER_REFRESH)
            {
                return true;
            }
            return false;
        }

        private bool HeadersRequireRefresh(string reqUri) {
            if (this.AppHeadersExist(reqUri)) {
                var uriHeaders = this.AllUriAppHeaders.First(entry => entry[HEADERS_TARGET] == reqUri);
                var lastModified = DateTime.Parse(uriHeaders[HEADERS_REFRESHED]);
                var minsSinceLastRefresh = DateTime.UtcNow.Subtract(lastModified).TotalMinutes;
                ////TODO: Can this call be async?
                this.DebugService.LogString("Minutes since last refresh: " + minsSinceLastRefresh.ToString(), LogOutputHelpers.ConsoleOnly());
                if (DateTime.UtcNow.Subtract(lastModified).TotalMinutes > AppConstants.MINUTES_PAST_REQUIRING_NEW_HEADER_REFRESH) {
                    return true;
                }
                else {
                    return false;
                }
            }
            return true;

        }

        /*
        public async Task<DateTime> GetLastModified(Uri reqUri)
        {
            if (this.HeadersRequireRefresh(reqUri.ToString()))
            {
                await this.PopulateHeadersCollection(reqUri);
            }
            else {
                this.GetAppHeaders(reqUri.ToString());
            }

            var result = Convert.ToDateTime(this.appHeaders[LAST_MODIFIED]).ToFileTimeUtc();
            return lastRefreshed;;
        }
        */

        public async Task<DateTimeOffset> GetLastModifiedHeaderTimeAsync(Uri reqUri)
        {
            await this.RefreshHeadersIfNecessary(reqUri);
            if (this.httpHeadersRetrieved.ContainsKey(this.UriToString(reqUri)))
            {
                return this.httpHeadersRetrieved[this.UriToString(reqUri)].Value;
            }
            return DateTimeOffset.MinValue;
        }

        private string UriToString(Uri uri)
        {
            return uri.AbsoluteUri.ToString();
        }

        /*
        public async Task<ulong> GetFilesize(Uri reqUri)
        {
            if (this.HeadersRequireRefresh(reqUri.ToString()))
            {
                await this.PopulateHeadersCollection(reqUri);
            }
            else {
                this.GetAppHeaders(reqUri.ToString());
            }

            var fileSize = Convert.ToUInt64(this.appHeaders[CONTENT_LENGTH]);
            return fileSize;
        }

        public void SetLastModified(Uri reqUri, DateTime mod) {
            this.GetAppHeaders(reqUri.ToString());

            this.appHeaders[LAST_MODIFIED] = mod.Ticks.ToString();
        }

        public void SetFilesize(Uri reqUri, ulong newSize) {
            this.GetAppHeaders(reqUri.ToString());
            this.appHeaders[CONTENT_LENGTH] = newSize.ToString();
        }
        */

    }
}
