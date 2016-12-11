using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    class EntityAttributeModifier
    {
        public EntityAttributeType AttributeType { get; }
        public int Value { get; }
        public Entity Source { get; }

        public EntityAttributeModifier(EntityAttributeType attributeType, int value, Entity source)
        {
            this.AttributeType = attributeType;
            this.Value = value;
            this.Source = source;
        }
    }
}
