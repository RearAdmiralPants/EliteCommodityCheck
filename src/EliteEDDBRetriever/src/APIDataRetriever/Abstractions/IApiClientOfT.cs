namespace EliteCommodityAnalysis.Abstractions {
    using System;
    using System.Threading.Tasks;

    /* This interface's boundaries begin when the application begins communicating with a third-party API and end when that communication is complete and the
        received data (if any) is queued to be processed into the application's main data structure(s). */
    public interface IApiClient<T> : IDisposable {
        Task StartAsync();

        Task StopAsync();

        // Intended to be a "cannot continue until the activity is stopped" fail-safe; may be a better implementation
        void Stop();

        T GetParameters();
    }
}