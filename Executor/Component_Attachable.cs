using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public class Component_Attachable : Component
    {
        private Entity attachedTo;

        public bool Attached { get { return this.attachedTo != null; } }
        public Entity AttachedTo { get { return this.attachedTo; } }
        public AttachmentSize SizeRequired { get; }

        public Component_Attachable(AttachmentSize sizeRequired)
        {
            this.attachedTo = null;
            this.SizeRequired = sizeRequired;
        }

        public void Notify_Attached(Entity attacher)
        {
            this.attachedTo = attacher;
        }

        public void Notify_Detached()
        {
            this.attachedTo = null;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            if (this.AttachedTo.GetComponentOfType<Component_AttachPoint>().Active)
                return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.ATTACHABLE)
                    .Add(SubEntitiesSelector.EQUIPPED);
            else
                return ImmutableHashSet<SubEntitiesSelector>.Empty.Add(SubEntitiesSelector.ATTACHABLE);
        }

        private void HandleQueryDestroyed(GameQuery_Destroyed q)
        {
            this.AttachedTo.HandleQuery(q);
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (q.AttributeType == EntityAttributeType.STRUCTURE && this.AttachedTo != null)
            {
                this.AttachedTo.HandleQuery(q);
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
