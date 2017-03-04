using System;
using Executor;
using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json;

namespace ExecutorTests
{
    [TestFixture]
    public class BlueprintListingTest
    {
        [Test]
        public void TestLoading()
        {
            // TODO: Put in loader
            // TODO: Test file in test project
            string blueprintsFile = "Resources/TestBlueprints.json";
            string text = System.IO.File.ReadAllText(blueprintsFile);

            var blueprint = JsonConvert.DeserializeObject<List<Blueprint>>(text)[0];
            Console.WriteLine(blueprint.Label);
            Console.WriteLine(blueprint.TypeLabel);
            foreach (var c in blueprint.Components)
            {
                Console.WriteLine(c.ComponentClass);
                foreach (var p in c.Params)
                {
                    Console.WriteLine(p);
                }
            }
            Assert.AreEqual(blueprint.Label, "a");
            Assert.AreEqual(blueprint.TypeLabel, "b");
            Assert.AreEqual("Executor.Component_Attachable", blueprint.Components[0].ComponentClass);
            Assert.AreEqual("Executor.AttachmentSize.LARGE", blueprint.Components[0].Params["sizeRequired"]);
            Assert.AreEqual("Executor.Component_Weapon", blueprint.Components[1].ComponentClass);
            Assert.AreEqual("5", blueprint.Components[1].Params["damage"]);

            var constructed = blueprint.BuildEntity();
            Assert.AreEqual("a", constructed.Label);
            var attachableComponent = constructed.GetComponentOfType<Component_Attachable>();
            Assert.AreEqual(AttachmentSize.LARGE, attachableComponent.SizeRequired);
            var weaponComponent = constructed.GetComponentOfType<Component_Weapon>();
            Assert.AreEqual(100, weaponComponent.WeaponAttributes[EntityAttributeType.REFIRE_TICKS]);
        }

        [Test]
        public void TestLoadsFromResources()
        {
            // TODO: Put in loader
            // TODO: Test file in test project
            BlueprintListing.LoadAllBlueprints();

            var powerPlant = BlueprintListing.GetBlueprintByLabel(Blueprints.POWER_PLANT);
            Assert.AreEqual("Pwr.Plnt.", powerPlant.Label);

            var machinegun = BlueprintListing.GetBlueprintByLabel(Blueprints.MACHINEGUN);
            Assert.AreEqual("Mchngn.", machinegun.Label);
        }
    }
}
