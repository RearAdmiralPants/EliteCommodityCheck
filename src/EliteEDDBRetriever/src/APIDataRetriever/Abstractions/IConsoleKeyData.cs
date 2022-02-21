namespace EliteCommodityAnalysis.Abstractions
{
    using System;

    public interface IConsoleKeyData
    {
        ConsoleKey KeyPressed { get; set; }

        DateTime KeyPressedTimestamp { get; set; }
    }
}