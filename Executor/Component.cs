using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public abstract class Component
    {
        private static IImmutableSet<Type> emptySet = ImmutableHashSet<Type>.Empty;

        private Entity parent;
        public Entity Parent { get { return this.parent; } }

        public IImmutableSet<SubEntitiesSelector> MatchingSelectors
        {
            get
            {
                return this._MatchingSelectors().Add(SubEntitiesSelector.ALL);
            }
        }

        public void Notify_Added(Entity attachingEntity)
        {
            foreach (var required in this._RequiredComponents())
            {
                if (!attachingEntity.HasComponentOfType(required))
                    throw new InvalidOperationException("Cannot add " + this + " to " + attachingEntity +
                        " missing required Component " + required);
            }
            foreach (var cannotAddAfter in this._CannotAddAfterComponents())
            {
                if (attachingEntity.HasComponentOfType(cannotAddAfter))
                    throw new InvalidOperationException("Cannot add " + this + " to " + attachingEntity +
                        " has Component " + cannotAddAfter + " before this component in the Entity!");
            }
            this.parent = attachingEntity;
        }

        public void Notify_Removed()
        {
            this.parent = null;
        }

        public GameEvent HandleEvent(GameEvent ev)
        {
            return this._HandleEvent(ev);
        }

        public GameQuery HandleQuery(GameQuery q)
        {
            return this._HandleQuery(q);
        }

        protected abstract IImmutableSet<SubEntitiesSelector> _MatchingSelectors();

        protected virtual IImmutableSet<Type> _CannotAddAfterComponents() { return Component.emptySet; }
        protected virtual IImmutableSet<Type> _RequiredComponents() { return Component.emptySet; }

        protected virtual GameEvent _HandleEvent(GameEvent ev) { return ev; }

        protected virtual GameQuery _HandleQuery(GameQuery q) { return q; }
    }
}
