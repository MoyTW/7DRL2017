using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class GameQuery_EntityAttribute : GameQuery
    {
        private bool registeredBase = false;
        private int baseValue = 0;
        private List<Tuple<int, Entity>> additiveModifiers = new List<Tuple<int, Entity>>();
        private List<Tuple<double, Entity>> multiplicativeModifiers = new List<Tuple<double, Entity>>();

        public Entity BaseEntity { get; }

        public EntityAttributeType AttributeType { get; }
        // TODO: Do we want to round up, round down, or round nearest? Right now rounds down.
        public int Value
        {
            get
            {
                if (this.BaseEntity != null && !this.registeredBase)
                    throw new InvalidOperationException("Base was never registered for - query for " +
                        this.AttributeType + " on " + this.BaseEntity);

                double value = this.baseValue + this.additiveModifiers.Sum(v => v.Item1);
                foreach(var multiplier in multiplicativeModifiers)
                {
                    value *= multiplier.Item1;
                }
                return (int)value;
            }
        }

        public GameQuery_EntityAttribute(EntityAttributeType attributeType) : this(attributeType, null) { }

        public GameQuery_EntityAttribute(EntityAttributeType attributeType, Entity baseEntity)
        {
            this.AttributeType = attributeType;
            this.BaseEntity = baseEntity;
        }

        public void RegisterBaseValue(int value)
        {
            if (this.BaseEntity == null)
                throw new InvalidOperationException("Can't register a base value if no base entity specified!");

            this.registeredBase = true;
            this.baseValue = value;
        }

        public void AddFlatModifier(int value, Entity source)
        {
            this.additiveModifiers.Add(new Tuple<int, Entity>(value, source));
        }

        public void AddMultModifier(double value, Entity source)
        {
            this.multiplicativeModifiers.Add(new Tuple<double, Entity>(value, source));
        }
    }
}
