
namespace EliteCommodityAnalysis.Abstractions.EDDB
{
    using Helpers;
    using SimpleLogger.Managers;
    using SimpleLogger.Abstractions;

    using EliteCommodityAnalysis.Abstractions;
    using EliteCommodityAnalysis.Abstractions.Overrides;
    using EliteCommodityAnalysis.Abstractions.LocalFiles;
    using EliteCommodityAnalysis.Abstractions.ApiParameters;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Simple asynchronous implementation of System.Net.webClient.DownloadFile to download "API" files from EDDB.
    /// </summary>
    public class EddbFileDownloader
    {
        private string rootDir = PerHostAppConstants.DEFAULT_ROOT_DIRECTORY;

        private HttpClient client;       
        private Stream clientStream = null;
        private BinaryReader RemoteStream = null;
        private FileStream LocalFile = null;
        private BinaryWriter LocalStream = null;
        private bool FileComplete = false;

        private HttpHeaderReceiver headerReceiver;

        private FileTypeToFilenameMapper filenameMapper;

        private Dictionary<string, ulong> FileSizes;
        private Dictionary<string, DateTime> ModificationTimes;
        private DateTimeOffset LastRemoteModified = DateTimeOffset.MinValue;

        private readonly IDebugService DebugService;
        
        private LogFileHelpers LogHelper;

        // public int BufferSize {get; set;} = 4194304; // 4MB too small
        public int BufferSize { get; set; } = 16777216;     // 16MB - //// TODO: Dynamically adjust based upon chunk download times

        public Dictionary<string, string> ApiParameters;

        public EddbParameters Parameters;

        public string RootDir
        {
            get
            {
                return this.rootDir;
            }
            set
            {
                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }
                this.rootDir = value;
            }
        }


        public EddbFileDownloader(HttpHeaderReceiver headerRecv, IDebugService debugService, IDebugMessage msg) {
            this.client = new HttpClient();
            this.headerReceiver = headerRecv;
            ////TODO: Finish application of DI here
            this.DebugService = debugService;
            ////TODO: Implement DI where applicable
            this.filenameMapper = new FileTypeToFilenameMapper();
            this.FileSizes = new Dictionary<string, ulong>();
            this.ModificationTimes = new Dictionary<string, DateTime>();
            // DI hack; only matters if we want to unit test the helpers themselves
            this.LogHelper = new LogFileHelpers(debugService, msg);
        }

        private void ReinitializeForNewFile() {
            this.client.Dispose();
            this.client = new HttpClient();
            if (this.clientStream != null) { this.clientStream.Dispose(); }
            this.clientStream = null;
        }

        // public IDictionary<string, string> GetApiParameters() {
        //     if (this.Parameters == null) { this.Parameters = this.GetParameters(); }
        //     return this.Parameters.GetParameters();
        // }

        public EddbParameters GetParameters() {
            return new EddbParameters();
        }

        private async Task<bool> AttemptDownloadOfFile(Uri target, string destDownload) {
            this.ReinitializeForNewFile();
            client.BaseAddress = new Uri("https://eddb.io");
            if (this.clientStream == null) {
                clientStream = await client.GetStreamAsync(target);
            }
            
            this.LocalFile = new FileStream(destDownload, FileMode.Create, FileAccess.Write, FileShare.None);
            this.LocalStream = new BinaryWriter(this.LocalFile);

            if (clientStream == null) {
                throw new ApplicationException("Something went horribly wrong");
            }
            this.RemoteStream = new BinaryReader(this.clientStream);
            this.FileComplete = false;
            await CompleteDownloadAndReportProgress(target, this.RemoteStream);

            this.clientStream.Close();

            return this.FileComplete;
        }

        //// TODO: Check status of bug where file to be downloaded has size less than buffer size
        private async Task CompleteDownloadAndReportProgress(Uri target, BinaryReader httpStream) {
            var currPosition = 0;
            while (!this.FileComplete) {
                await this.ReadNextChunk(currPosition, this.BufferSize, target.ToString());
                if (this.FileComplete) {
                    await this.DebugService.LogStringAsync("File complete.", LogOutputHelpers.ConsoleOnly());
                }
                else {
                    var total = await this.headerReceiver.GetFilesize(target);
                    await this.DebugService.LogStringAsync("Out of " + total.ToString() + ", " + this.LocalFile.Position.ToString() + " bytes are retrieved.", LogOutputHelpers.ConsoleOnly());
                }
            }
            
        }

        /// <summary>
        /// Reads the next chunk of the file and writes to disk if necessary.
        /// </summary>
        /// <param name="currPosition"></param>
        /// <param name="bufferSize"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private async Task ReadNextChunk(int currPosition, int bufferSize, string target) {
            var flushes = 0;
            //// TODO: BUG here - if the file size is less than the chunk size, we consider it a failed download.
            var anticipatedProgress = Convert.ToUInt64(this.LocalFile.Position + bufferSize);
            var fileSize = await this.headerReceiver.GetFilesize(new Uri(target));
            await this.DebugService.LogStringAsync("Anticipated progress: " + anticipatedProgress.ToString() + "; total size: " + fileSize.ToString(), LogOutputHelpers.ConsoleOnly());
            if (anticipatedProgress < fileSize) {
                await this.DebugService.LogStringAsync("Retrieving " + this.BufferSize.ToString() + " bytes...", LogOutputHelpers.ConsoleOnly());
                var bufferData = this.RemoteStream.ReadBytes(this.BufferSize);
                if (bufferData.Length < this.BufferSize) {
                    await this.LogHelper.LogStringToConsoleAsync("Buffer filled incompletely - assuming end-of-stream, despite header information.");
                    this.LocalStream.Write(bufferData);
                    this.LocalStream.Close();
                    this.FileComplete = true;
                }
                else {
                    this.LocalStream.Write(bufferData);
                    flushes++;
                    if (flushes > 32) {
                        this.LocalStream.Flush();
                        flushes = 0;
                    }
                }
            }
            else {
                await this.DebugService.LogStringAsync("Final retrieval...", LogOutputHelpers.ConsoleOnly());
                var totalSize = await this.headerReceiver.GetFilesize(new Uri(target));
                var bufferData = this.RemoteStream.ReadBytes(Convert.ToInt32(totalSize - Convert.ToUInt64(this.LocalFile.Position)));
                this.LocalStream.Write(bufferData);
                this.LocalStream.Close();
                this.FileComplete = true;
            }

        }

        // Retrieves a given file from EDDB if the rules say it should be refreshed or retrieved.
        public async Task<bool> GetFileIfNecessary(Uri target, string destDownload)
        {
            await this.DebugService.LogStringAsync("Getting " + target.ToString() + " if necessary...", LogOutputHelpers.ConsoleOnly());            
            
            var dlResult = false;
            var retrieve = false;
            // sanity check
            //var lastModified = await this.headerReceiver.GetLastModified(target);
            var lastModified = await this.headerReceiver.GetLastModifiedHeaderTimeAsync(target);
            var minsSinceRemoteChanged = lastModified.Subtract(this.LastRemoteModified);
            this.LastRemoteModified = lastModified;

            //if (this.LastRemoteModified != lastModified)
            //{
            //    await this.DebugService.LogStringAsync("Last Modified header has never been set or has changed; downloading.");
            //    retrieve = true;
            //    this.LastRemoteModified = lastModified;
            //}
            //if (lastModified == DateTimeOffset.MinValue || lastModified == DateTime.MaxValue) {
            //    await this.DebugService.LogStringAsync("Last Modified header has never been set; downloading.");
            //    retrieve = true;
            //}
            if (File.Exists(destDownload)) {
                var info = new FileInfo(destDownload);
                var apiSize = await this.headerReceiver.GetFilesize(target);
                var localSize = Convert.ToUInt64(info.Length);
                if (localSize != uint.MaxValue && localSize != apiSize) {
                    await this.DebugService.LogStringAsync("For " + destDownload + ", sizes differ on local (" + localSize.ToString() + ") vs. API size (" + apiSize.ToString(), LogOutputHelpers.ConsoleOnly());
                    if (!retrieve) { retrieve = true; }
                }
                else
                {
                    var localLastModification = info.LastWriteTimeUtc;
                    var apiLastModification = lastModified.UtcDateTime;
                    if (apiLastModification.Subtract(localLastModification).TotalMinutes >= AppConstants.MINUTES_PAST_REQUIRING_NEW_DOWNLOAD)
                    {
                        await this.DebugService.LogStringAsync("Local file's last write time (" + info.LastWriteTime.ToString() + ") in UTC (" + localLastModification.ToString() + ") differs from API's reported last modification (" + lastModified.ToString() + ") by " + apiLastModification.Subtract(localLastModification).TotalMinutes.ToString() + " minutes. File will be retrieved.", LogOutputHelpers.ConsoleOnly());
                        retrieve = true;
                    }
                    else
                    {
                        await this.DebugService.LogStringAsync("Local file's last write time (" + info.LastWriteTime.ToString() + ") in UTC (" + localLastModification.ToString() + ") differs from API's reported last modification (" + lastModified.ToString() + ") by " + apiLastModification.Subtract(localLastModification).TotalMinutes.ToString() + " minutes. File **NEED NOT** be retrieved.", LogOutputHelpers.ConsoleOnly());
                        if (!retrieve)
                        {
                            await this.DebugService.LogStringAsync("**NO DOWNLOAD REQUIRED**", LogOutputHelpers.ConsoleOnly());
                        }
                    }
                }
            }
            else {
                await this.DebugService.LogStringAsync("API file does not exist locally; downloading.");

                retrieve = true;
            }

//           var minsSinceModified = InitializedDateTime.NowMinusPastDateTime(lastModified).TotalMinutes;
            //if (minsSinceRemoteChanged.TotalMinutes >= AppConstants.MINUTES_PAST_REQUIRING_NEW_DOWNLOAD || retrieve) {
            
            //    await this.DebugService.LogStringAsync("For " + destDownload + " timespan since last modified time is " + minsSinceRemoteChanged.ToString() + " or " + minsSinceRemoteChanged.TotalMinutes.ToString() + " minutes greater than the local version.", LogOutputHelpers.ConsoleOnly());
            //    await this.DebugService.LogStringAsync("New download required for " + target.ToString() + " to " +  destDownload, LogOutputHelpers.ConsoleOnly());
            //    retrieve = true;
            //}
            //else {
            //    await this.DebugService.LogStringAsync("No download required.", LogOutputHelpers.ConsoleOnly());
            //}
            if (retrieve) { dlResult = await this.AttemptDownloadOfFile(target, destDownload); }
            if (dlResult) {
                // Modify file modify time to match web modify time
                File.SetLastWriteTime(destDownload, lastModified.LocalDateTime);

                var debugInfo = new FileInfo(destDownload);
                await this.DebugService.LogStringAsync("Local modified time is now: " + debugInfo.LastWriteTime.ToString() + " (UTC: " + debugInfo.LastWriteTimeUtc.ToString() + ") after setting the last write time to the LOCAL DateTime (" + lastModified.LocalDateTime.ToString() + ")", LogOutputHelpers.ConsoleOnly());
                var debugApiModTime = await this.headerReceiver.GetLastModifiedHeaderTimeAsync(target);
                await this.DebugService.LogStringAsync("Remote modified time is: " + debugApiModTime.ToString(), LogOutputHelpers.ConsoleOnly());
            }
            return dlResult;
        }
    }
}
