using System;

namespace MechArena
{
    [Serializable()]
    public class Component
    {
        private Entity parent;
        public Entity Parent { get { return this.parent; } }

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

        protected virtual GameEvent _HandleEvent(GameEvent ev) { return ev; }

        protected virtual GameQuery _HandleQuery(GameQuery q) { return q; }
    }
}
