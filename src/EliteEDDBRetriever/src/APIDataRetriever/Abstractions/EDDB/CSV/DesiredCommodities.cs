namespace APIDataRetriever.Abstractions.EDDB.CSV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    /// <summary>
    /// This class may not be used in release; for now, it specifies the commodities whose price history we are interested in (e.g. Platinum)
    /// </summary>
    public static class DesiredCommodities
    {
        public static IEnumerable<string> GetDesiredCommodities()
        {
            return new HashSet<string>() { "Platinum", "Osmium" };
        }
    }
}
