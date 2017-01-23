using System;
using System.Collections.Generic;

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

        public bool HasMountedEntity { get { return this.mountedEntity != null; } }
        public MountSize MaxSize { get; }
        public bool Swappable { get; }

        public Component_Mount(MountSize maxSize, bool swappable)
        {
            this.mountedEntity = null;
            this.MaxSize = maxSize;
            this.Swappable = swappable;
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

        protected override ISet<SubEntitiesSelector> _MatchingSelectors()
        {
            var selectors = new HashSet<SubEntitiesSelector>();
            if (this.Swappable)
                selectors.Add(SubEntitiesSelector.SWAPPABLE_MOUNTS);
            return selectors;
        }

        private void HandleSlot(GameEvent_Slot ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                if (this.CanMount(ev.EntityToSlot))
                {
                    this.mountedEntity = ev.EntityToSlot;
                    ev.EntityToSlot.GetComponentOfType<Component_Mountable>().Notify_Mounted(this.Parent);
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
                ev.EntityToUnslot.GetComponentOfType<Component_Mountable>().Notify_Unmounted();
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
            // Structure is not passed to Mountable!
            if (q.AttributeType != EntityAttributeType.STRUCTURE &&
                this.mountedEntity != null &&
                !this.mountedEntity.TryGetDestroyed())
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
