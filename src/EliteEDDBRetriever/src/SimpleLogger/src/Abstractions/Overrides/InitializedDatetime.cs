namespace SimpleLogger.Abstractions.Overrides
{
    using System;

    public static class InitializedDateTime
    {
        public static DateTime GetPastDatetime()
        {
            // use some static random class to get some number of days in the past
            return new DateTime(1981, 7, 22);
        }

        public static DateTime GetFutureDatetime()
        {
            // use some static random class to get some number of days in the future
            return new DateTime(DateTime.UtcNow.Year + 50, 3, 10);
        }

        public static TimeSpan NowMinusArbitraryPastDateTime()
        {
            var pastTime = GetPastDatetime();
            return DateTime.UtcNow.Subtract(pastTime);
        }

        public static TimeSpan NowPlusRArbitraryFutureDateTime()
        {
            var futureTime = GetFutureDatetime();
            return futureTime.Subtract(DateTime.UtcNow);
        }

        public static TimeSpan NowMinusPastDateTime(DateTime input) {
            return DateTime.UtcNow.Subtract(input.ToUniversalTime());
        }
    }
}
