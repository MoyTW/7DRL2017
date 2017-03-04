using System;
using System.Collections.Immutable;
using System.Linq;

namespace Executor
{
    public enum AttachmentSize
    {
        SMALL = 0,
        MEDIUM = 1,
        LARGE = 2
    }

    [Serializable()]
    public class Component_AttachPoint : Component
    {
        private Entity attachedEntity;

        public bool HasAttachedEntity { get { return this.attachedEntity != null; } }
        public AttachmentSize MaxSize { get; }
        public bool Active { get; }
        public bool Swappable { get; }

        public Component_AttachPoint(AttachmentSize maxSize, bool active, bool swappable)
        {
            this.attachedEntity = null;
            this.MaxSize = maxSize;
            this.Active = active;
            this.Swappable = swappable;
        }

        public Entity InspectAttachedEntity()
        {
            return this.attachedEntity;
        }

        public bool CanAttach(Entity en)
        {
            return this.attachedEntity == null &&
                this.MaxSize >= en.GetComponentOfType<Component_Attachable>().SizeRequired;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            if (this.Swappable)
                return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.SWAPPABLE_ATTACH_POINTS)
                    .Add(SubEntitiesSelector.ATTACH_POINT);
            else
                return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.ATTACH_POINT);
        }

        private void HandleSlot(GameEvent_Slot ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                if (this.CanAttach(ev.EntityToSlot))
                {
                    this.attachedEntity = ev.EntityToSlot;
                    ev.EntityToSlot.GetComponentOfType<Component_Attachable>().Notify_Attached(this.Parent);
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
                if (this.attachedEntity != ev.EntityToUnslot)
                    throw new ArgumentException("Cannot unmount unmounted item " + ev.EntityToUnslot.ToString() + "!");

                this.attachedEntity = null;
                ev.EntityToUnslot.GetComponentOfType<Component_Attachable>().Notify_Detached();
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
            if (this.attachedEntity == null)
                return;

            if (q.MatchesSelectors(this.attachedEntity) ||
                q.Selectors.Contains(SubEntitiesSelector.ACTIVE_TRACKS_TIME) &&
                this.Active &&
                this.attachedEntity.MatchesSelector(SubEntitiesSelector.TRACKS_TIME))
            {
                q.RegisterEntity(this.attachedEntity);
            }
            this.attachedEntity.HandleQuery(q);
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            // Attached items should not contribute to structure!
            if (q.AttributeType != EntityAttributeType.STRUCTURE &&
                this.attachedEntity != null &&
                !this.attachedEntity.TryGetDestroyed())
            {
                this.attachedEntity.HandleQuery(q);
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
