using System;
using System.Collections.Generic;

namespace MechArena
{
    [Serializable()]
    class Component_Mountable : Component
    {
        private Entity mountedTo;

        public bool Mounted { get { return this.mountedTo != null; } }
        public Entity MountedTo { get { return this.mountedTo; } }
        public MountSize SizeRequired { get; }

        public Component_Mountable(MountSize sizeRequired)
        {
            this.mountedTo = null;
            this.SizeRequired = sizeRequired;
        }

        public void Notify_Mounted(Entity mounter)
        {
            this.mountedTo = mounter;
        }

        public void Notify_Unmounted()
        {
            this.mountedTo = null;
        }

        protected override ISet<SubEntitiesSelector> _MatchingSelectors()
        {
            return new HashSet<SubEntitiesSelector>();
        }

        private void HandleQueryDestroyed(GameQuery_Destroyed q)
        {
            this.MountedTo.HandleQuery(q);
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (q.AttributeType == EntityAttributeType.STRUCTURE && this.MountedTo != null)
            {
                this.MountedTo.HandleQuery(q);
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Destroyed)
                this.HandleQueryDestroyed((GameQuery_Destroyed)q);
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);

            return q;
        }
    }
}
