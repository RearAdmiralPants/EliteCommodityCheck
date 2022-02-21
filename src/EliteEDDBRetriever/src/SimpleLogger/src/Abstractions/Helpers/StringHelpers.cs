namespace SimpleLogger.Abstractions.Helpers
{
    using System;
    public class StringHelpers
    {
        // Returns a US-style string representation of the current timezone's date (Month/Day/Year), e.g. 07/22/1524
        public static string TimezoneNowToString() {
            var now = DateTime.Now;
            return now.Day.ToString("D2") + "/" + now.Month.ToString("D2") + "/" + now.Year.ToString("D4");
        }

        // Returns a US-style string representation of the current UTC date (Month/Day/Year), e.g. 07/22/1524
        public static string UtcNowToString() {
            var now = DateTime.UtcNow;
            return now.Day.ToString("D2") + "/" + now.Month.ToString("D2") + "/" + now.Year.ToString("D4");
        }

        public static string TimezoneNowToFilesafeString() {
            return TimezoneNowToString().Replace("/", "-");
        }

        public static string UtcNowToFilesafeString() {
            return UtcNowToString().Replace("/", "-");
        }
    }
}
