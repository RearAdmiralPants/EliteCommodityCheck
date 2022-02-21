namespace EliteCommodityAnalysis.Abstractions.EDDB
{
    using System;
    /// <summary>
    /// Maps EDDB API file types to the URLs from which they may be retrieved.
    /// </summary>
    /// <remarks>
    /// Code smell: String constants, but we have no control over EDDB
    /// </remarks>
    public class EddbFileTypeToUrlMapper
    {
        public static string GetUrl(ApiFileType eddbType)
        {
            if (eddbType == ApiFileType.Systems)
            {
                return "https://eddb.io/archive/v6/systems_populated.json";
            }
            if (eddbType == ApiFileType.Commodities)
            {
                return "https://eddb.io/archive/v6/commodities.json";
            }
            if (eddbType == ApiFileType.Stations)
            {
                return "https://eddb.io/archive/v6/stations.json";
            }
            if (eddbType == ApiFileType.Listings)
            {
                return "https://eddb.io/archive/v6/listings.csv";
            }
            if (eddbType == ApiFileType.Factions) {
                return "https://eddb.io/archive/v6/factions.json";
            }
            if (eddbType == ApiFileType.Modules) {
                return "https://eddb.io/archive/v6/modules.json";
            }
            throw new ArgumentException("Invalid EDDB API object type.");
        }
    }
}
