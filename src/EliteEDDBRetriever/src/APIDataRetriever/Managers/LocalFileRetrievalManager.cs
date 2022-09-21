namespace EliteCommodityAnalysis.Managers {
    using System;
    using System.IO;
    using System.Timers;

    using EliteCommodityAnalysis.Abstractions.EDDB;
    using EliteCommodityAnalysis.Abstractions.LocalFiles;
    using EliteCommodityAnalysis.Abstractions;
    using System.Threading;
    using System.Threading.Tasks;

    using SimpleLogger.Managers;
    using SimpleLogger.Abstractions;
    using EliteCommodityAnalysis.Abstractions.Helpers;

    public class LocalFileRetrievalManager
    {
        /// A string representing the root of the data directory such that:
        ///     -- this.dataRoowDirectory
        ///     |
        ///     ---- [Date Directory, e.g. "20200502" for May 2, 2020]
        ///         |
        ///         -- API file 1
        ///         -- API file 2
        ///         -- ...
        ///     ---- [Date Directory]
        ///         |
        ///         -- API file 1
        ///         -- API file 2
        ///         -- etc.
        private string dataRootDirectory = null;

        /// A reference to the class that does the work to download a single file and provide the progress of each download
        ////TODO: May not be required
        private EddbFileDownloader downloader = null;

        /// A reference to the class that retrieves and caches headers for a given API file
        private HttpHeaderReceiver headerReceiver = null;

        /// A reference to the class that maps API file types (stations, commodities, listings) to local filenames.
        private FileTypeToFilenameMapper localMapper = null;

        // DI semi-hacks for log helpers (LOW priority: investigate a better way)
        private IDebugService DebugService;

        private IDebugMessage DebugMessage;

        private CancellationTokenSource CancelSource = new CancellationTokenSource();
        private CancellationToken CancellationToken;

        public bool SuccessfullyCanceled {get; private set;} = false;

        public Exception FailureException { get; private set; } = null;

        private bool updatingLocalFiles = false;

        private int MainLoopIntervalSeconds = 600;      ////TODO: Reset to appropriate interval prior to release
        private int DelayChunkSeconds = 5;

        private uint mainLoopExecutions = 0;

        //// TODO: Figure out more nuanced DI if it is even required
        public LocalFileRetrievalManager(string rootDir, IDebugService debugService, IDebugMessage debugMessage, HttpHeaderReceiver rcvr, EddbFileDownloader fileDownloader)
        {
            this.DebugService = debugService;
            this.DebugMessage = debugMessage;
            this.headerReceiver = rcvr;
            this.dataRootDirectory = rootDir;
            this.downloader = fileDownloader;
            this.localMapper = new FileTypeToFilenameMapper();
        }

        public LocalFileRetrievalManager(IDebugService debugService, IDebugMessage debugMessage, HttpHeaderReceiver rcvr, EddbFileDownloader fileDownloader)
        {
            this.DebugService = debugService;
            this.DebugMessage = debugMessage;
            this.headerReceiver = rcvr;
            this.downloader = fileDownloader;
            this.localMapper = new FileTypeToFilenameMapper();
        }

        public async Task Run() {
            //this.CancellationToken = new CancellationToken(false);
            this.CancellationToken = this.CancelSource.Token;
            await this.StartAsync(this.CancellationToken);
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            var loopSpan = TimeSpan.FromSeconds(this.MainLoopIntervalSeconds);
            var infoMsg = "EDDB API file retrieval beginning. Attempts will be made every ";
            if (loopSpan.TotalMinutes < 1)
            {
                infoMsg += loopSpan.TotalSeconds.ToString() + " seconds.";
            }
            else if (loopSpan.TotalHours < 1)
            {
                infoMsg += loopSpan.TotalMinutes.ToString() + " minutes.";
            }
            else if (loopSpan.TotalDays < 1)
            {
                infoMsg += loopSpan.TotalHours.ToString() + " hours.";
            }
            else
            {
                infoMsg += loopSpan.TotalDays.ToString() + " days.";
            }
            infoMsg += " To cancel, press ESC, wait for a message to press a key, and do so to gracefully shut down.";
            Console.WriteLine(infoMsg);

            await this.RepeatActionEvery(() => {
                uint thisLoops = 0;
                if (!this.updatingLocalFiles) {
                    this.CheckForNewFiles();
                    //if (this.mainLoopExecutions > thisLoops)
                    //{
                        thisLoops = this.mainLoopExecutions;
                    //}
                }
            }, TimeSpan.FromSeconds(this.MainLoopIntervalSeconds));

            Console.WriteLine("Escaped from main loop.");
        }

        private async Task RepeatActionEvery(Action action, TimeSpan interval) {
            this.CancellationToken.ThrowIfCancellationRequested();
            var delayChunk = new TimeSpan(0, 0, this.DelayChunkSeconds);
            var chunkDelayTask = Task.Delay(delayChunk, this.CancellationToken);
            var chunksInInterval = interval.Divide(delayChunk);
            int delayChunksWaited = 0;
            while (!this.CancellationToken.IsCancellationRequested) {

                if (this.HasEscBeenPressed())
                {
                    this.RequestCancellation();
                }
                try {
                    if (this.FailureException != null) {
                        // Indicates a failure condition from which we cannot recover.
                        return;
                    }

                    action();

                    if (!this.CancellationToken.IsCancellationRequested || delayChunksWaited >= chunksInInterval) {
                        //var delay = Task.Delay(interval, this.CancellationToken);
                        //await delay;
                        delayChunksWaited = 0;
                        while (delayChunksWaited < chunksInInterval)
                        {
                            chunkDelayTask = Task.Delay(delayChunk, this.CancellationToken);
                            await chunkDelayTask;
                            delayChunksWaited++;
                            Console.WriteLine("Waited " + delayChunksWaited.ToString() + " chunks / " + chunksInInterval.ToString() + "...");
                            if (this.HasEscBeenPressed())
                            {
                                this.RequestCancellation();
                            }
                        }
                    }
                }
                catch (TaskCanceledException) {
                    // while loop will end anyway; is this code necessary?
                    this.SuccessfullyCanceled = true;
                    return;
                }
                catch (OperationCanceledException) {
                    this.SuccessfullyCanceled = true;
                    return;
                }
                catch (AggregateException ex) {
                    this.FailureException = ex;
                    return;
                }
                catch (Exception ex) {
                    this.FailureException = ex;
                    return;
                }
            }
            this.SuccessfullyCanceled = true;
        }

        public void RequestCancellation() {
            this.CancelSource.Cancel();
        }

        private void CheckForNewFiles()
        {

            var helper = new LogFileHelpers(this.DebugService, this.DebugMessage);
            helper.LogStringToConsole("Checking for new files...");

            var catchTask = Task.Run(async () =>
            {
                await this.UpdateLocalFiles();
                this.mainLoopExecutions++;
            });
            var exTask = catchTask.ContinueWith(catcher => {
                this.FailureException = catcher.Exception;
                // Could determine what kind of exception is happening here, but it's likely an IOException preventing creation of the required files that will receive the API's contents for the day
                
                // No need to throw the exception - in fact, it's not caught in a try/catch block outside of the ContinueWith() task anyway
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private bool HasEscBeenPressed() {
            if (Console.KeyAvailable) {
                if (Console.ReadKey(true).Key == ConsoleKey.Escape) {
                    return true;
                }
            }
            return false;
        }

        private void CreateLocalPathIfNeeded(string path)
        {
            if (!Path.IsPathFullyQualified(path)) {
                // Need to use Path.GetFullPath(string, string)
                // In the interest of cross-platform, this is not implemented yet
                throw new NotImplementedException("Relative paths are not currently supported due to cross-platform concerns. They will be addressed in a future version.");
            }
            var justDir = Path.GetFullPath(path);
            var justFile = Path.GetFileName(path);
            var createDir = justDir.Substring(0, path.Length - justFile.Length);
            if (!Directory.Exists(createDir))
            {
                Directory.CreateDirectory(createDir);
            }
        }

            public async Task UpdateLocalFiles()
        {
            if (this.updatingLocalFiles) { 
                var logHelper = new Abstractions.Helpers.LogFileHelpers(this.DebugService, this.DebugMessage);
                await logHelper.LogStringToConsoleAsync("A previous iteration is already running. This iteration of the loop will do nothing.\r\n*********** THIS SHOULD NOT OCCUR WITH THE NEW TASK-BASED TIMERLESS WORKFLOW ***************");
                return; 
            }
            this.updatingLocalFiles = true;
            foreach (var enumValue in Enum.GetValues(typeof(ApiFileType))) {
                var apiFileTypeVal = (int)enumValue;
                if (apiFileTypeVal < 100 || PerHostAppConstants.GET_UNCHANGED_FILES) {
                    var apiFileType = (ApiFileType)enumValue;

                    var fileUrl = EddbFileTypeToUrlMapper.GetUrl(apiFileType);
                    var fileUri = new Uri(fileUrl);

                    var destDownload = this.localMapper.GetLocalFilename(apiFileType);
                    this.CreateLocalPathIfNeeded(destDownload);

                    ////TODO: BUG: If we do not yet have the file for the day but it was just posted by the webserver,
                    ////    the minutes since the HTTP server's Last-Modified header are low enough that we choose not to
                    ////    download it. We need to be careful that the file's age (check Created header?) is newer than
                    ////    the last file we downloaded, and if so, override the behavior and download the file anyway.
                    ////    BELIEVED to be solved - requires testing
                    var result = await this.downloader.GetFileIfNecessary(fileUri, destDownload);

                    if (result == false) {
                    // No download required / got NULL bytes from server.
                    }
                    else {
                    // File Downloaded!
                    }
                }
            }
            this.updatingLocalFiles = false;
        }
    }
}
