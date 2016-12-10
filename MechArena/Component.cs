using System;

namespace MechArena
{
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

        public GameEvent HandleEvent(GameEvent e)
        {
            return this._HandleEvent(e);
        }

        public GameQuery HandleQuery(GameQuery q)
        {
            return this._HandleQuery(q);
        }

        protected virtual GameEvent _HandleEvent(GameEvent e)
        {
            throw new NotImplementedException();
        }

        protected virtual GameQuery _HandleQuery(GameQuery q)
        {
            throw new NotImplementedException();
        }
    }
}
