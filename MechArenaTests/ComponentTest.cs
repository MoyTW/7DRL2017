using MechArena;
using System.Collections.Immutable;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests
{
    [TestClass]
    public class ComponentTest
    {
        private class NeutralComponent : Component
        {
            protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
            {
                return ImmutableHashSet<SubEntitiesSelector>.Empty;
            }
        }

        private class CannotAddAfterNeutralComponent : Component
        {
            protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
            {
                return ImmutableHashSet<SubEntitiesSelector>.Empty;
            }

            protected override IImmutableSet<Type> _CannotAddAfterComponents()
            {
                return ImmutableHashSet<Type>.Empty.Add(typeof(NeutralComponent));
            }
        }

        private class RequiresNeutralComponent : Component
        {
            protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
            {
                return ImmutableHashSet<SubEntitiesSelector>.Empty;
            }

            protected override IImmutableSet<Type> _RequiredComponents()
            {
                return ImmutableHashSet<Type>.Empty.Add(typeof(NeutralComponent));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCannotAddAfter()
        {
            new Entity().AddComponent(new NeutralComponent()).AddComponent(new CannotAddAfterNeutralComponent());
        }

        [TestMethod]
        public void TestCanAddBefore()
        {
            var e = new Entity().AddComponent(new CannotAddAfterNeutralComponent()).AddComponent(new NeutralComponent());
            Assert.IsTrue(e.HasComponentOfType<CannotAddAfterNeutralComponent>());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRequiresThrowsIfNotPresent()
        {
            new Entity().AddComponent(new RequiresNeutralComponent());
        }

        [TestMethod]
        public void TestRequiresWorksIfPresent()
        {
            var e = new Entity().AddComponent(new NeutralComponent()).AddComponent(new RequiresNeutralComponent());
            Assert.IsTrue(e.HasComponentOfType<NeutralComponent>());
        }
    }
}
