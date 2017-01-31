using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public static class BlueprintListing
    {
        //TODO: Place this in a configuration file
        public const string BlueprintsDir = "Resources/Blueprints";

        public static IEnumerable<Blueprint> LoadAllBlueprints()
        {
            var blueprints = new List<Blueprint>();

            var files = Directory.GetFiles(BlueprintsDir);
            foreach(var f in files)
            {
                string text = File.ReadAllText(f);
                var deserializedBlueprints = JsonConvert.DeserializeObject<List<Blueprint>>(text);
                blueprints.AddRange(deserializedBlueprints);
            }

            return blueprints;
        }
    }
}
