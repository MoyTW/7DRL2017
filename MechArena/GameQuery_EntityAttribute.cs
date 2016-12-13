using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class GameQuery_EntityAttribute : GameQuery
    {
        private List<EntityAttributeModifier> modifiers;

        public EntityAttributeType AttributeType { get; }
        public int Value
        {
            get
            {
                return this.modifiers.Sum(m => m.Value);
            }
        }

        public GameQuery_EntityAttribute(EntityAttributeType attributeType)
        {
            this.AttributeType = attributeType;
            this.modifiers = new List<EntityAttributeModifier>();
        }

        public void AddModifier(int value, Entity source)
        {
            this.modifiers.Add(new EntityAttributeModifier(this.AttributeType, value, source));
        }
    }
}
