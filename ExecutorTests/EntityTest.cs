using Executor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ExecutorTests
{
    class TestComponent : Component
    {
        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }
    }

    [TestFixture]
    public class EntityTest
    {
        private Entity ent;
        private Component cmp;

        [SetUp()]
        public void Initialize()
        {
            this.ent = new Entity();
            this.cmp = new TestComponent();
            BlueprintListing.LoadAllBlueprints();
        }

        [Test]
        public void TestAddComponent()
        {
            this.ent.AddComponent(this.cmp);
            Assert.AreEqual(1, this.ent.InspectComponents().Count);
            Assert.AreEqual(this.cmp, this.ent.InspectComponents()[0]);
        }

        [Test]
        public void TestAddComponentDouble()
        {
            this.ent.AddComponent(this.cmp);
            Assert.Throws<ArgumentException>(() => this.ent.AddComponent(this.cmp));
        }

        [Test]
        public void TestRemoveComponentThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => this.ent.RemoveComponent(this.cmp));
        }

        [Test]
        public void TestRemoveComponent()
        {
            this.ent.AddComponent(this.cmp);
            this.ent.RemoveComponent(this.cmp);
        }

        /*
        [Test]
        public void TestDeepCopy()
        {
            var mech = EntityBuilder.BuildArmoredMech("test", false);
            Entity deepCopy = mech.DeepCopy();

            Assert.AreEqual(mech.EntityID, deepCopy.EntityID);

            Assert.AreEqual(mech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value,
                deepCopy.TryGetAttribute(EntityAttributeType.STRUCTURE).Value);

            var damageEvent = new GameEvent_TakeDamage(10);
            mech.GetComponentOfType<Component_Skeleton>().InspectBodyPart(BodyPartLocation.TORSO)
                .HandleEvent(damageEvent);
            Assert.AreNotEqual(mech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value,
                deepCopy.TryGetAttribute(EntityAttributeType.STRUCTURE).Value);
        }
        */
    }
}
