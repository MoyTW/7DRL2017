using RogueSharp.Random;

using Executor;

using System;
using NUnit.Framework;

namespace ExecutorTests
{
    [TestFixture]
    public class Component_SlottedStructureTest
    {
        int slotSpace = 5;
        int structureMax = 10;
        Entity bodyPart;

        private Entity SlotEntityWithStructure(Entity container, int structure, int slotsRequired=1)
        {
            var en = new Entity();
            en.AddComponent(new Component_Slottable(slotsRequired));
            en.AddComponent(new Component_InternalStructure(structure));

            var ev = new GameEvent_Slot(container, container, en);
            container.HandleEvent(ev);

            return en;
        }

        [SetUp()]
        public void Initialize()
        {
            this.bodyPart = new Entity();
            this.bodyPart
                .AddComponent(new Component_SlottedContainer(this.slotSpace))
                .AddComponent(new Component_SlottedStructure())
                // TODO: Fall-through should not be tested here!
                .AddComponent(new Component_InternalStructure(this.structureMax));
        }

        // TODO: Fall-through should not be tested here!
        [Test]
        public void TestTakeDamageWithNoSlottedEntities()
        {
            var ev = new GameEvent_TakeDamage(3);
            this.bodyPart.HandleEvent(ev);
            Assert.AreEqual(0, ev.DamageRemaining);
            Assert.IsTrue(ev.Completed);
            Assert.AreEqual(7,
                this.bodyPart.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
        }

        [Test]
        public void TestTakeDamageWithSlottedEntities()
        {
            Entity slotted = this.SlotEntityWithStructure(this.bodyPart, 5);

            var ev = new GameEvent_TakeDamage(3);
            this.bodyPart.HandleEvent(ev);
            Assert.AreEqual(0, ev.DamageRemaining);
            Assert.IsTrue(ev.Completed);
            Assert.AreEqual(this.structureMax,
                this.bodyPart.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
            Assert.AreEqual(2, slotted.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
        }

        // Probibalistic failure possible!
        [Test]
        public void TestAssignsDamageToLargestSlottedEntity()
        {
            Entity tinySlotted = this.SlotEntityWithStructure(this.bodyPart, 3);
            Entity hugeSlotted = this.SlotEntityWithStructure(this.bodyPart, 9999);

            var ev = new GameEvent_TakeDamage(100);
            this.bodyPart.HandleEvent(ev);
            Assert.AreEqual(0, ev.DamageRemaining);
            Assert.IsTrue(ev.Completed);
            Assert.AreEqual(this.structureMax,
                this.bodyPart.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
            Assert.AreEqual(3, tinySlotted.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
            Assert.AreEqual(9899, hugeSlotted.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
        }

        // TODO: Fall-through should not be tested here!
        [Test]
        public void TestTakeDamageFallsThrough()
        {
            Entity tinySlotted = this.SlotEntityWithStructure(this.bodyPart, 5);

            var ev = new GameEvent_TakeDamage(100);
            this.bodyPart.HandleEvent(ev);
            Assert.AreEqual(85, ev.DamageRemaining);
            Assert.IsFalse(ev.Completed);
            Assert.AreEqual(0,
                this.bodyPart.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
            Assert.AreEqual(0, tinySlotted.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
        }
    }
}
