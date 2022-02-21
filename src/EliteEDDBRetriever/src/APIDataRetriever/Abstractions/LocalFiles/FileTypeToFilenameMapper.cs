namespace EliteCommodityAnalysis.Abstractions.LocalFiles
{
    using System;
    using System.IO;

    ////TODO: This could be a static class
    public class FileTypeToFilenameMapper
    {
        private string rootDir = PerHostAppConstants.DEFAULT_ROOT_DIRECTORY;
        private bool downloadsOcurring = false;

        private string GetFilename(ApiFileType eddbType)
        {
            var dayDirString = GenerateDateDirectoryString();
            if (eddbType == ApiFileType.Systems)
            {
                return rootDir + dayDirString + "systems_populated.json";
            }
            if (eddbType == ApiFileType.Commodities)
            {
                return rootDir + dayDirString + "commodities.json";
            }
            if (eddbType == ApiFileType.Stations)
            {
                return rootDir + dayDirString + "stations.json";
            }
            if (eddbType == ApiFileType.Listings)
            {
                return rootDir + dayDirString + "listings.csv";
            }
            if (eddbType == ApiFileType.Factions) {
                return rootDir + "factions.json";
            }
            if (eddbType == ApiFileType.Modules) {
                return rootDir + "modules.json";
            }
            throw new ArgumentException("Invalid Local File object type.");
        }

        public static string GenerateDateDirectoryString() {
            var utcNow = DateTime.UtcNow;

            var output = "";

            output += utcNow.Year.ToString("D4");
            output += utcNow.Month.ToString("D2");
            output += utcNow.Day.ToString("D2");
            output += AppConstants.DIRECTORY_SEPARATOR;

            return output;
        }

        public string RootDir
        {
            get
            {
                return this.rootDir;
            }
            set
            {
                if (!this.downloadsOcurring && !Directory.Exists(value))
                {
                    this.rootDir = value;
                }
            }
        }

        public string GetLocalFilename(ApiFileType fileType) {
            return this.GetFilename(fileType);
        }
    }
}
