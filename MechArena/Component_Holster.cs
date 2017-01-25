using System;
using System.Collections.Immutable;
using System.Linq;

namespace MechArena
{
    [Serializable()]
    class Component_Holster : Component
    {
        private Entity holsteredEntity;

        public bool HasHolsteredEntity { get { return this.holsteredEntity != null; } }
        public MountSize MaxSize { get; }

        public Component_Holster(MountSize maxSize)
        {
            this.holsteredEntity = null;
            this.MaxSize = maxSize;
        }

        public Entity InspectHolsteredEntity()
        {
            return this.holsteredEntity;
        }

        public bool CanHolster(Entity en)
        {
            return this.holsteredEntity == null &&
                this.MaxSize >= en.GetComponentOfType<Component_Mountable>().SizeRequired;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.HOLSTERS);
        }

        private void HandleSlot(GameEvent_Slot ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                if (this.CanHolster(ev.EntityToSlot))
                {
                    this.holsteredEntity = ev.EntityToSlot;
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
                if (this.holsteredEntity != ev.EntityToUnslot)
                    throw new ArgumentException("Cannot unmount unmounted item " + ev.EntityToUnslot.ToString() + "!");

                this.holsteredEntity = null;
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
            if (this.holsteredEntity == null)
                return;

            if (q.MatchesSelectors(this.holsteredEntity) ||
                q.Selectors.Contains(SubEntitiesSelector.DISABLED_TRACKS_TIME) &&
                this.holsteredEntity.MatchesSelector(SubEntitiesSelector.TRACKS_TIME))
            {
                q.RegisterEntity(this.holsteredEntity);
            }
            this.holsteredEntity.HandleQuery(q);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_SubEntities)
                this.HandleQuerySubEntities((GameQuery_SubEntities)q);

            return q;
        }
    }
}
