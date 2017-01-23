using MechArena;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MechArenaTests
{
    class TestComponent : Component
    {
        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }
    }

    [TestClass]
    public class EntityTest
    {
        private Entity ent = new Entity();
        private Component cmp = new TestComponent();

        [TestMethod]
        public void TestAddComponent()
        {
            this.ent.AddComponent(this.cmp);
            Assert.AreEqual(1, this.ent.InspectComponents().Count);
            Assert.AreEqual(this.cmp, this.ent.InspectComponents()[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddComponentDouble()
        {
            this.ent.AddComponent(this.cmp);
            this.ent.AddComponent(this.cmp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveComponentThrows()
        {
            this.ent.RemoveComponent(this.cmp);
        }

        [TestMethod]
        public void TestRemoveComponent()
        {
            var a = new List<String>();
            this.ent.AddComponent(this.cmp);
            this.ent.RemoveComponent(this.cmp);
        }

        [TestMethod]
        public void TestDeepCopy()
        {
            var mech = EntityBuilder.BuildArmoredMech("test", false);
            Entity deepCopy = mech.DeepCopy();

            Assert.AreEqual(mech.EntityID, deepCopy.EntityID);

            Assert.AreEqual(mech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value,
                deepCopy.TryGetAttribute(EntityAttributeType.STRUCTURE).Value);

            mech.HandleEvent(new GameEvent_TakeDamage(9999, new RogueSharp.Random.DotNetRandom()));
            Assert.AreNotEqual(mech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value,
                deepCopy.TryGetAttribute(EntityAttributeType.STRUCTURE).Value);
        }
    }
}
