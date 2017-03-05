using Executor;

using System;
using NUnit.Framework;

namespace ExecutorTests
{
    // I don't know how to design in OO any more! The fact that I have one test class for 4(!) different classes is a
    // serious smell!
    [TestFixture]
    public class Component_SlottedContainerTest
    {
        int containerSize = 5;
        Entity slottedContainer;

        private Entity SlottableRequiring(int slotsRequired)
        {
            Entity e = new Entity();
            e.AddComponent(new Component_Slottable(slotsRequired));
            return e;
        }

        [SetUp()]
        public void Initialize()
        {
            this.slottedContainer = new Entity();
            this.slottedContainer.AddComponent(new Component_SlottedContainer(this.containerSize));
        }

        [Test]
        public void CanSlot()
        {
            var small = this.SlottableRequiring(2);
            var ev = new GameEvent_Slot(this.slottedContainer, this.slottedContainer, small);
            this.slottedContainer.HandleEvent(ev);
            Assert.IsTrue(ev.Completed);
            Assert.AreEqual(3, this.slottedContainer.GetComponentOfType<Component_SlottedContainer>().SlotsRemaining);

            var perfect = this.SlottableRequiring(3);
            var ev1 = new GameEvent_Slot(this.slottedContainer, this.slottedContainer, perfect);
            this.slottedContainer.HandleEvent(ev1);
            Assert.IsTrue(ev1.Completed);
            Assert.AreEqual(0, this.slottedContainer.GetComponentOfType<Component_SlottedContainer>().SlotsRemaining);
        }

        [Test]
        public void CannotDoubleSlot()
        {
            var small = this.SlottableRequiring(2);
            var ev = new GameEvent_Slot(this.slottedContainer, this.slottedContainer, small);
            this.slottedContainer.HandleEvent(ev);
			var slotEvent = new GameEvent_Slot (this.slottedContainer, this.slottedContainer, small);
			Assert.Throws<ArgumentException> (() => this.slottedContainer.HandleEvent(slotEvent));
        }

        [Test]
        public void CanUnslot()
        {
            var small = this.SlottableRequiring(2);
            var ev = new GameEvent_Slot(this.slottedContainer, this.slottedContainer, small);
            this.slottedContainer.HandleEvent(ev);
            
            var unslot = new GameEvent_Unslot(this.slottedContainer, this.slottedContainer, small);
            this.slottedContainer.HandleEvent(unslot);
            Assert.IsTrue(unslot.Completed);
            Assert.AreEqual(this.containerSize,
                this.slottedContainer.GetComponentOfType<Component_SlottedContainer>().SlotsRemaining);
        }

        [Test]
        public void CannotUnslotIfUnattached()
        {
            var small = this.SlottableRequiring(2);
            var unslot = new GameEvent_Unslot(null, this.slottedContainer, small);
			Assert.Throws<ArgumentException>(() => this.slottedContainer.HandleEvent(unslot));
        }
    }
}
