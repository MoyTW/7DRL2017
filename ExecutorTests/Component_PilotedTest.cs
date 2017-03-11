using Executor;

using System;
using NUnit.Framework;

namespace ExecutorTests
{
    /*
    [TestFixture]
    public class Component_PilotedTest
    {
        private Entity pilot, mech1, mech1Gun, mech2;
        private ArenaState arena;

        [SetUp]
        public void Initialize()
        {
            BlueprintListing.LoadAllBlueprints();

            this.pilot = new Entity();

            this.mech1 = EntityBuilder.BuildNakedMech("1", true, pilot, null);
            this.mech1Gun = BlueprintListing.BuildForLabel(Blueprints.PISTOL);
            EntityBuilder.MountOntoArm(this.mech1, BodyPartLocation.RIGHT_ARM, mech1Gun);

            this.mech2 = EntityBuilder.BuildArmoredMech("2", false);
            this.arena = ArenaBuilder.TestArena(mech1, mech2);
        }

        [Test]
        public void TestAppliesToWeapons()
        {
            pilot.AddComponent(new Component_AttributeModifier(EntityAttributeType.DAMAGE, ModifierType.FLAT, 4));

            var stub = new CommandStub_PrepareTargetedAttack(mech1.EntityID, mech2.EntityID, mech2.Label, 
                BodyPartLocation.TORSO);
            var attack = stub.ReifyStub(this.arena);
            mech1.HandleEvent(attack);

            Assert.IsTrue(attack.LogMessage.Contains("6 dmg"));
        }

        [Test]
        public void TestAppliesOnlyToSpecificBaseLabel()
        {
            pilot.AddComponent(new Component_AttributeModifier(EntityAttributeType.DAMAGE, ModifierType.FLAT, 4,
                "Pstl."));
            Assert.AreEqual(6, this.mech1.TryGetAttribute(EntityAttributeType.DAMAGE, this.mech1Gun).Value);

            pilot.AddComponent(new Component_AttributeModifier(EntityAttributeType.DAMAGE, ModifierType.FLAT, 4,
                "Swrd."));
            Assert.AreEqual(6, this.mech1.TryGetAttribute(EntityAttributeType.DAMAGE, this.mech1Gun).Value);

            pilot.AddComponent(new Component_AttributeModifier(EntityAttributeType.DAMAGE, ModifierType.FLAT, 4));
            Assert.AreEqual(10, this.mech1.TryGetAttribute(EntityAttributeType.DAMAGE, this.mech1Gun).Value);
        }
    }
    */
}

