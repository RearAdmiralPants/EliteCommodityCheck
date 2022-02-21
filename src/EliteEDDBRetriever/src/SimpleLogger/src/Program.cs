namespace SimpleLogger
{
    using System;
    using System.Threading.Tasks;

    using Abstractions;
    using Managers;
    using Drivers;

    using Microsoft.Extensions.DependencyInjection;
    class Program
    {
        private static ServiceProvider _serviceProvider = null;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Registering DI services...");
            RegisterServices();
            IServiceScope scope = _serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<DebugLoggerRunner>().Run();
            Console.WriteLine("Press a key to dispose and exit...");
            Console.ReadKey();
            DisposeServices();
        }

        private static void RegisterServices() {
            var services = new ServiceCollection();
            services.AddTransient<IDebugMessage, DebugMessage>();
            services.AddSingleton<IDebugService, DebugService>();
            services.AddSingleton<DebugLoggerRunner>();
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void DisposeServices() {
            if (_serviceProvider == null) { return; }

            if (_serviceProvider is IDisposable) {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
