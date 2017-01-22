using System;
using System.Collections.Generic;

namespace MechArena
{
    [Serializable()]
    public abstract class Component
    {
        private Entity parent;
        public Entity Parent { get { return this.parent; } }
        public ISet<SubEntitiesSelector> MatchingSelectors
        {
            get
            {
                ISet<SubEntitiesSelector> derivedSelectors = this._MatchingSelectors();
                derivedSelectors.Add(SubEntitiesSelector.ALL);
                return derivedSelectors;
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

        protected abstract ISet<SubEntitiesSelector> _MatchingSelectors();

        protected virtual GameEvent _HandleEvent(GameEvent ev) { return ev; }

        protected virtual GameQuery _HandleQuery(GameQuery q) { return q; }
    }
}
