namespace EliteCommodityAnalysis
{    
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using EliteCommodityAnalysis.Abstractions;
    using EliteCommodityAnalysis.Abstractions.EDDB;
    using EliteCommodityAnalysis.Abstractions.LocalFiles;
    using Managers;
    using Managers.EDDB;

    using SimpleLogger.Abstractions;
    using SimpleLogger.Managers;
    public class Program
    {
        private static ServiceProvider _serviceProvider = null;

        public static async Task Main(string[] args)
        {
            // Debugging
            
            /*
            Mapping.MapperConfig.Configure();
            var parser = new EDDBDataParsingManager();
            /*
            //var path = @"F:\src\EDDBData\20210206\commodities.json";
            var path = PerHostAppConstants.COMMODITIES_EDDB_JSON_FILE_PATH;
            await parser.ParseCommodities(path);

            //var statPath = @"F:\src\EDDBData\20210206\stations.json";
            var statPath = PerHostAppConstants.STATIONS_EDDB_JSON_FILE_PATH;
            await parser.ParseStations(statPath);
            */

            /*
            var sysPath = PerHostAppConstants.SYSTEMS_EDDB_JSON_FILE_PATH;
            await parser.ParseSystems(sysPath);
            return;
            // 
            */

            try {
            RegisterServices();
            IServiceScope scope = _serviceProvider.CreateScope();
            var retrievalManager = scope.ServiceProvider.GetRequiredService<LocalFileRetrievalManager>();

            Console.WriteLine("Press ESC to stop.");
            
            await retrievalManager.Run();

            while (!retrievalManager.SuccessfullyCanceled && retrievalManager.FailureException == null) {
                Console.WriteLine("Awaiting successful cancellation...");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            if (retrievalManager.SuccessfullyCanceled) {
                Console.WriteLine("Cancellation successful. Shutting down.");
            }
            else if (retrievalManager.FailureException != null) {
                Console.WriteLine("One or more exceptions occurred; shutting down: ");
                Console.WriteLine(retrievalManager.FailureException.ToString());
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
            Console.WriteLine("Press a key to shut down and end program.");
            Console.ReadKey();
            }
            catch (Exception ex) {
                Console.WriteLine("Unhandled exception during application main loop: " + ex.ToString());
            }
            finally {
                Console.WriteLine("Cleaning up for graceful exit...");
                DisposeServices();
            }
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<IDebugMessage, DebugMessage>();
            services.AddSingleton<IDebugService, DebugService>();
            services.AddSingleton<LocalFileRetrievalManager>();
            ////TODO: Analyze the scopes here and determine if there are any potential pitfalls with reusing the same object (as singleton)
            services.AddSingleton<HttpHeaderReceiver>();
            services.AddSingleton<EddbFileDownloader>();
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null) { return; }

            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
