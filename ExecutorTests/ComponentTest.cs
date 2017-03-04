using Executor;
using System.Collections.Immutable;
using System;
using NUnit.Framework;

namespace ExecutorTests
{
    [TestFixture]
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

        [Test]
        public void TestCannotAddAfter()
        {
			var e = new Entity ().AddComponent (new NeutralComponent ());
			Assert.Throws<InvalidOperationException> (() =>
				e.AddComponent(new CannotAddAfterNeutralComponent()));
        }

        [Test]
        public void TestCanAddBefore()
        {
            var e = new Entity().AddComponent(new CannotAddAfterNeutralComponent()).AddComponent(new NeutralComponent());
            Assert.IsTrue(e.HasComponentOfType<CannotAddAfterNeutralComponent>());
        }

        [Test]
        public void TestRequiresThrowsIfNotPresent()
        {
			Assert.Throws<InvalidOperationException>( () =>
				new Entity().AddComponent(new RequiresNeutralComponent()));
        }

        [Test]
        public void TestRequiresWorksIfPresent()
        {
            var e = new Entity().AddComponent(new NeutralComponent()).AddComponent(new RequiresNeutralComponent());
            Assert.IsTrue(e.HasComponentOfType<NeutralComponent>());
        }
    }
}
