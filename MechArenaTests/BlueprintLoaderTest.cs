using System;
using MechArena;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace MechArenaTests
{
    [TestClass]
    public class BlueprintLoaderTest
    {
        [TestMethod]
        public void TestLoading()
        {
            // TODO: Put in loader
            // TODO: Test file in test project
            string blueprintsFile = "Resources/TestBlueprints.json";
            string text = System.IO.File.ReadAllText(blueprintsFile);

            var blueprint = JsonConvert.DeserializeObject<List<Blueprint>>(text)[0];
            Console.WriteLine(blueprint.Label);
            Console.WriteLine(blueprint.TypeLabel);
            foreach(var c in blueprint.Components)
            {
                Console.WriteLine(c.Class);
                foreach(var p in c.Params)
                {
                    Console.WriteLine(p);
                }
            }
            Assert.AreEqual(blueprint.Label, "a");
            Assert.AreEqual(blueprint.TypeLabel, "b");
            Assert.AreEqual(blueprint.Components[0].Class, "Component_Attachable");
            Assert.AreEqual(blueprint.Components[0].Params["sizeRequired"], "LARGE");
            Assert.AreEqual(blueprint.Components[1].Class, "Component_Weapon");
            Assert.AreEqual(blueprint.Components[1].Params["damage"], "5");
        }
    }
}
