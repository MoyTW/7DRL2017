using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class GameQuery_EntityAttribute : GameQuery
    {
        private List<Tuple<int, Entity>> additiveModifiers = new List<Tuple<int, Entity>>();
        private List<Tuple<double, Entity>> multiplicativeModifiers = new List<Tuple<double, Entity>>();

        public EntityAttributeType AttributeType { get; }
        // TODO: Do we want to round up, round down, or round nearest? Right now rounds down.
        public int Value
        {
            get
            {
                double value = this.additiveModifiers.Sum(v => v.Item1);
                foreach(var multiplier in multiplicativeModifiers)
                {
                    value *= multiplier.Item1;
                }
                return (int)value;
            }
        }

        public GameQuery_EntityAttribute(EntityAttributeType attributeType)
        {
            this.AttributeType = attributeType;
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
