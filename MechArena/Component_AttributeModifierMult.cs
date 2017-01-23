using System;
using System.Collections.Immutable;

namespace MechArena
{
    [Serializable()]
    class Component_AttributeModifierMult : Component
    {
        public EntityAttributeType AttributeType { get; }
        public double Value { get; }

        public Component_AttributeModifierMult(EntityAttributeType attributeType, double value)
        {
            this.AttributeType = attributeType;
            this.Value = value;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (q.AttributeType == this.AttributeType)
                q.AddMultModifier(this.Value, this.Parent);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);

            return q;
        }
    }
}
