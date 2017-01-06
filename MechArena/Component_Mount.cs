using System;

namespace MechArena
{
    public enum MountSize
    {
        SMALL = 0,
        MEDIUM = 1,
        LARGE = 2
    }

    [Serializable()]
    class Component_Mount : Component
    {
        private Entity mountedEntity;

        public MountSize MaxSize { get; }

        public Component_Mount(MountSize maxSize)
        {
            this.mountedEntity = null;
            this.MaxSize = maxSize;
        }

        public Entity InspectMountedEntity()
        {
            return this.mountedEntity;
        }

        public bool CanMount(Entity en)
        {
            return this.mountedEntity == null &&
                this.MaxSize >= en.GetComponentOfType<Component_Mountable>().SizeRequired;

        }

        private void HandleSlot(GameEvent_Slot ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                if (this.CanMount(ev.EntityToSlot))
                {
                    this.mountedEntity = ev.EntityToSlot;
                    ev.Completed = true;
                }
                else
                {
                    throw new InvalidOperationException("Can't mount item " + ev.EntityToSlot + "!");
                }
            }
        }

        private void HandleUnslot(GameEvent_Unslot ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                if (this.mountedEntity != ev.EntityToUnslot)
                    throw new ArgumentException("Cannot unmount unmounted item " + ev.EntityToUnslot.ToString() + "!");

                this.mountedEntity = null;
                ev.Completed = true;
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_Slot)
                this.HandleSlot((GameEvent_Slot)ev);
            else if (ev is GameEvent_Unslot)
                this.HandleUnslot((GameEvent_Unslot)ev);

            return ev;
        }

        private void HandleQuerySubEntities(GameQuery_SubEntities q)
        {
            if (this.mountedEntity != null)
            {
                if (q.MatchesSelectors(this.mountedEntity))
                    q.RegisterEntity(this.mountedEntity);
                this.mountedEntity.HandleQuery(q);
            }
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (this.mountedEntity != null && !this.mountedEntity.TryGetDestroyed())
            {
                this.mountedEntity.HandleQuery(q);
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);
            else if (q is GameQuery_SubEntities)
                this.HandleQuerySubEntities((GameQuery_SubEntities)q);

            return q;
        }
    }
}
