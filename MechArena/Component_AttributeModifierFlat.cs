using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    class Component_AttributeModifierFlat : Component
    {
        public EntityAttributeType AttributeType { get; }
        public int Value { get; }

        public Component_AttributeModifierFlat(EntityAttributeType attributeType, int value)
        {
            this.AttributeType = attributeType;
            this.Value = value;
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
