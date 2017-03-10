using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    public static class Blueprints
    {
        public const string POWER_PLANT = "Pwr.Plnt.";
        public const string HAND = "Hand";
        public const string LEG_ACTUATOR = "Lg.Actr.";
        public const string RIFLE = "Rfl.";
        public const string MACHINEGUN = "Mchngn.";
        public const string PISTOL = "Pstl.";
        public const string SMALL_MOUNT = "Smll.Mnt.";
        public const string MEDIUM_MOUNT = "Mdm.Mnt.";
        public const string LARGE_MOUNT = "Lrg.Mnt.";
        public const string SMALL_HOLSTER = "Smll.Hlstr.";
        public const string MEDIUM_HOLSTER = "Mdm.Hlstr.";
        public const string LARGE_HOLSTER = "Lrg.Hlstr.";
        public const string ACCELERATOR = "Acclrtr.";
        public const string SENSOR_PACKAGE = "Snsr.Pckg.";
        public const string ARMOR = "Armor";
    }

    public static class BlueprintListing
    {
        private static Dictionary<string, Blueprint> labelsToBlueprints = new Dictionary<string, Blueprint>();

        //TODO: Place this in a configuration file
        public const string BlueprintsDir = "Resources/Blueprints";

        public static void LoadAllBlueprints(string blueprintsDir=BlueprintListing.BlueprintsDir)
        {
            var files = Directory.GetFiles(blueprintsDir);
            foreach (var f in files)
            {
                string text = File.ReadAllText(f);
                var deserializedBlueprints = JsonConvert.DeserializeObject<List<Blueprint>>(text);
                foreach (var bp in deserializedBlueprints)
                {
                    BlueprintListing.labelsToBlueprints[bp.Label] = bp;
                }
            }
        }

        public static Blueprint GetBlueprintByLabel(string label)
        {
            return BlueprintListing.labelsToBlueprints[label];
        }

        public static IEnumerable<Blueprint> GetAllBlueprints()
        {
            return BlueprintListing.labelsToBlueprints.Values;
        }

        public static Entity BuildForLabel(string label)
        {
            return BlueprintListing.GetBlueprintByLabel(label).BuildEntity();
        }

        public static IEnumerable<Blueprint> GetMatchingBlueprints(SubEntitiesSelector selector)
        {
            return BlueprintListing.GetAllBlueprints().Where(b => b.MatchesSelector(selector));
        }
    }
}
