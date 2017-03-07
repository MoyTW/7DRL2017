using Executor;

using System;
using NUnit.Framework;

namespace ExecutorTests
{
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
            var mountedGun = EntityBuilder.BuildMountedPistol(mech1, BodyPartLocation.HEAD);
            this.mech1Gun = mountedGun.GetComponentOfType<Component_AttachPoint>().InspectAttachedEntity();
            EntityBuilder.SlotAt(mech1, BodyPartLocation.HEAD, mountedGun);
            EntityBuilder.BuildMountedRifle(mech1, BodyPartLocation.LEFT_ARM);

            this.mech2 = EntityBuilder.BuildNakedMech("2", false, new Entity(), null);
            this.arena = ArenaBuilder.TestArena(mech1, mech2);
        }

        [Test]
        public void TestAppliesToWeapons()
        {
            pilot.AddComponent(new Component_AttributeModifier(EntityAttributeType.DAMAGE, ModifierType.FLAT, 4));

            var stub = new CommandStub_PrepareAttack(mech1.EntityID, mech2.EntityID, BodyPartLocation.TORSO);
            var attack = stub.ReifyStub(this.arena);
            mech1.HandleEvent(attack);

            // Assert.AreEqual(attack.ResultDamage, 6);
            Assert.AreEqual(0, 1);
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
}

