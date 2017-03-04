using RogueSharp.Random;
using Executor;

using System;
using NUnit.Framework;

namespace ExecutorTests
{
    [TestFixture]
    public class Component_InternalStructureTest
    {
        int structureMax = 10;
        Entity internalStructure;

        private Entity SlottableRequiring(int slotsRequired)
        {
            Entity e = new Entity();
            e.AddComponent(new Component_Slottable(slotsRequired));
            return e;
        }

        [SetUp()]
        public void Initialize()
        {
            this.internalStructure = new Entity();
            this.internalStructure.AddComponent(new Component_InternalStructure(this.structureMax));
        }

        [Test]
        public void CanTakeDamage()
        {
            var ev = new GameEvent_TakeDamage(3);
            this.internalStructure.HandleEvent(ev);
            Assert.AreEqual(0, ev.DamageRemaining);
            Assert.IsTrue(ev.Completed);
            Assert.AreEqual(7,
                this.internalStructure.GetComponentOfType<Component_InternalStructure>().StructureRemaining);

            var ev1 = new GameEvent_TakeDamage(5);
            this.internalStructure.HandleEvent(ev1);
            Assert.AreEqual(0, ev1.DamageRemaining);
            Assert.IsTrue(ev1.Completed);
            Assert.AreEqual(2,
                this.internalStructure.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
        }

        [Test]
        public void CanTakeExactDamage()
        {
            var ev = new GameEvent_TakeDamage(10);
            this.internalStructure.HandleEvent(ev);
            Assert.AreEqual(0, ev.DamageRemaining);
            Assert.IsTrue(ev.Completed);
            Assert.AreEqual(0,
                this.internalStructure.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
        }

        [Test]
        public void CanTakeDamageOverflowsProperly()
        {
			var ev = new GameEvent_TakeDamage(20);
            this.internalStructure.HandleEvent(ev);
            Assert.AreEqual(10, ev.DamageRemaining);
            Assert.IsFalse(ev.Completed);
            Assert.AreEqual(0,
                this.internalStructure.GetComponentOfType<Component_InternalStructure>().StructureRemaining);
        }
    }
}
