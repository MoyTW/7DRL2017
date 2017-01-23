using System;
using System.Collections.Immutable;

namespace MechArena
{
    [Serializable()]
    class Component_AttributeModifierFlat : Component
    {
        public EntityAttributeType AttributeType { get; }
        public int Value { get; }

        public Component_AttributeModifierFlat(EntityAttributeType attributeType, int value)
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
                q.AddFlatModifier(this.Value, this.Parent);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);

            return q;
        }
    }
}
