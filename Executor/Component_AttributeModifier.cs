using System;
using System.Collections.Immutable;

namespace Executor
{
    public enum ModifierType
    {
        FLAT,
        MULTIPLIER
    }

    [Serializable()]
    public class Component_AttributeModifier : Component
    {
        public EntityAttributeType AttributeType { get; }
        public ModifierType ModifierType { get; }
        public double Value { get; }
        public string RequiredBaseLabel { get; }

        public Component_AttributeModifier(EntityAttributeType attributeType, ModifierType modifierType,
            double value)
        {
            this.AttributeType = attributeType;
            this.ModifierType = modifierType;
            this.Value = value;
        }

        public Component_AttributeModifier(EntityAttributeType attributeType, ModifierType modifierType,
            double value, string requiredBaseLabel=null)
        {
            this.AttributeType = attributeType;
            this.ModifierType = modifierType;
            this.Value = value;
            this.RequiredBaseLabel = requiredBaseLabel;
        }


        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (q.AttributeType == this.AttributeType &&
                (this.RequiredBaseLabel == null || this.RequiredBaseLabel == q.BaseEntity.Label))
            {
                if (this.ModifierType == ModifierType.FLAT)
                    q.AddFlatModifier(this.Value, this.Parent);
                else
                    q.AddMultModifier(this.Value, this.Parent);
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);

            return q;
        }
    }
}
