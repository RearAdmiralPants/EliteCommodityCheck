namespace EliteCommodityAnalysis.Abstractions
{
    using System;
    using System.Collections.Concurrent;
    public interface IConsoleKeysPressed : IDisposable
    {
        //// SEMI-HACK: Ideally the setter would only be accessible to the collection of assemblies in the solution; to do that, we would need inherited interfaces e.g. IConsoleKeysPressedInternal : IConsoleKeysPressed with
        //// an internal setter, while the setter would be removed from this interface. Too much work for the level of testing and third-party dependencies (zero?) using this solution.
        // Gets or sets a representation of the collection of keys pressed during runtime on the console.
        ConcurrentBag<IConsoleKeyData> KeysPressed { get; set; }
    }
}