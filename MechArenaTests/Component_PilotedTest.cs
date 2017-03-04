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

            this.mech1 = EntityBuilder.BuildNakedMech("1", false, pilot, null);
            var mountedGun = EntityBuilder.BuildMountedPistol();
            this.mech1Gun = mountedGun.GetComponentOfType<Component_AttachPoint>().InspectAttachedEntity();
            EntityBuilder.SlotAt(mech1, BodyPartLocation.HEAD, mountedGun);
            EntityBuilder.SlotAt(mech1, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildMountedRifle());
            EntityBuilder.SlotAt(mech1, BodyPartLocation.RIGHT_ARM, EntityBuilder.BuildMountedSword());

            this.mech2 = EntityBuilder.BuildNakedMech("2", false, new Entity(), null);
            this.arena = ArenaBuilder.TestArena(0, mech1, mech2);
        }

        [Test]
        public void TestAppliesToWeapons()
        {
            pilot.AddComponent(new Component_AttributeModifier(EntityAttributeType.DAMAGE, ModifierType.FLAT, 4))
                .AddComponent(new Component_AttributeModifier(EntityAttributeType.TO_HIT, ModifierType.FLAT, 100));

            var attack = new GameEvent_Attack(0, mech1, mech2, this.mech1Gun, arena.ArenaMap, arena.SeededRand);
            mech1.HandleEvent(attack);

            Assert.IsTrue(attack.ResultHit);
            Assert.AreEqual(attack.ResultDamage, 6);
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

