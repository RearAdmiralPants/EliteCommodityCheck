namespace EliteCommodityAnalysis.Abstractions.Helpers
{
    using System;
    public class StringHelpers
    {
        public string TimezoneNowToString() {
            var now = DateTime.Now;
            return now.Day.ToString("D2") + "/" + now.Month.ToString("D2") + "/" + now.Year.ToString("D4");
        }

        public string UtcNowToString() {
            var now = DateTime.UtcNow;
            return now.Day.ToString("D2") + "/" + now.Month.ToString("D2") + "/" + now.Year.ToString("D4");
        }
    }
}
