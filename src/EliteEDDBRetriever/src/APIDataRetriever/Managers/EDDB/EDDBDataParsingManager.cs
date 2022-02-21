namespace EliteCommodityAnalysis.Managers.EDDB {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Newtonsoft.Json;
    using Mapster;

    using Abstractions;
    using Abstractions.EDDB;
    using Abstractions.EDDB.JSON;

    using System.Reflection;

    // This manager will hand off tasks to the two EDDB parsers; the end result should be either calls to our API or shortcuts directly to our repository

    ////TODO: populate e.g. Station.Commodities using the Commodities.ID field, etc.
    ////TODO: Dependency Injection!
    public class EDDBDataParsingManager {
        public List<Commodity> Commodities { get; private set; } = new List<Commodity>();
        public List<Station> Stations {get; private set; } = new List<Station>();

        public List<PopulatedSystem> PopulatedSystems {get; private set; } = new List<PopulatedSystem>();
        
        public EDDBDataParsingManager() { }

        //// TODO: Use .NET Core's JSON serializer - better than Newtonsoft
        // Parses the commodities JSON file from EDDB and projects it into our model
        public async Task ParseCommodities(string filename) {
            this.Commodities.Clear();

            var json = await File.ReadAllTextAsync(filename);
            JSONCommodity[] commodities = JsonConvert.DeserializeObject<JSONCommodity[]>(json);

            var dest = commodities.Adapt<Commodity[]>();

            this.Commodities.AddRange(dest);
        }

        // Parses the Stations JSON file from EDDB and projects it into our model
        public async Task ParseStations(string filename) {
            
            var json = await File.ReadAllTextAsync(filename);
            JSONStation[] stations = JsonConvert.DeserializeObject<JSONStation[]>(json);

            Console.WriteLine("JSON parsed!");

            var dest = stations.Adapt<Station[]>();

            this.Stations.AddRange(dest);

            Console.WriteLine("Mapped " + this.Stations.Count.ToString() + " stations!");
        }

        public async Task ParseSystems(string filename) {
            var json = await File.ReadAllTextAsync(filename);
            JSONPopulatedSystem[] systems = JsonConvert.DeserializeObject<JSONPopulatedSystem[]>(json);

            Console.WriteLine("JSON parsed!");

            Console.WriteLine(systems.Length.ToString() + " systems deserialized into JSON objects and sub-objects.");
            
            var dest = systems.Adapt<PopulatedSystem[]>();

            this.PopulatedSystems.AddRange(dest);

            Console.WriteLine("Mapped " + this.PopulatedSystems.Count.ToString() + " systems.");

            var deciat = this.PopulatedSystems.FirstOrDefault(s => s.Name.ToLowerInvariant() == "deciat");
            /*
            var stringPropertyNamesAndValues = myObject.GetType()
    .GetProperties()
    .Where(pi => pi.PropertyType == typeof(string) && pi.GetGetMethod() != null)
    .Select(pi => new 
    {
        Name = pi.Name,
        Value = pi.GetGetMethod().Invoke(myObject, null)
    });
    */
            foreach (var prop in deciat.GetType().GetProperties()) {
                //this.PropsOut(prop.GetValue(deciat));
                // if (prop.GetValue(deciat)?.GetType().GetProperties().Length > 0) {
                    if (prop != null) {
                        Console.WriteLine(prop.Name + ": " + prop.GetValue(deciat)?.ToString());
                    }
                // }
            }
            Console.ReadKey();

        }
/*
        public void PropsOut(object obj) {
            if (obj == null) { Console.WriteLine( "NULL property "); 
            return;
            }
            try {
            if (obj.GetType().GetProperties() == null) { return; }
            foreach (var prop in obj.GetType().GetProperties()) {
                if (prop.GetValue(obj) == null) {
                    Console.WriteLine(prop.Name + ": NULL");
                    return;
                }
/*                if (prop.GetValue(obj)?.GetType().GetProperties()?.Length > 0) {
                    this.PropsOut(prop.GetValue(obj));        
                }
                else {
                    Console.WriteLine(prop.Name + ": ");
                }
 
            }
            catch (Exception) {
                Console.WriteLine("Oops");
            }
        }
        */

  }
}