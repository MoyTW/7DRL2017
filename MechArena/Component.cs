using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace MechArena
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

        public void Notify_Added(Entity parent)
        {
            this.parent = parent;
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

        protected virtual GameEvent _HandleEvent(GameEvent ev) { return ev; }

        protected virtual GameQuery _HandleQuery(GameQuery q) { return q; }
    }
}
