using System;
using System.Collections.Generic;

namespace MechArena
{
    public enum WeaponSize
    {
        SMALL = 0,
        MEDIUM,
        LARGE
    }

    [Serializable()]
    public class Component_Weapon : Component_TracksTime
    {
        public WeaponSize Size { get; }
        private readonly Dictionary<EntityAttributeType, int> WeaponAttributes;

        // TODO: I would like to define a construct which can be read from a file to construct these!
        // Right now I'll just hard-code them all and feel bad about it.
        public Component_Weapon(WeaponSize size, int toHit, int maxRange, int damage, int refireTicks)
            : base(EntityAttributeType.REFIRE_TICKS)
        {
            this.Size = size;
            this.WeaponAttributes = new Dictionary<EntityAttributeType, int>() {
                { EntityAttributeType.TO_HIT, toHit },
                { EntityAttributeType.MAX_RANGE, maxRange },
                { EntityAttributeType.DAMAGE, damage },
                { EntityAttributeType.REFIRE_TICKS, refireTicks },
            };
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (this.WeaponAttributes.ContainsKey(q.AttributeType))
                q.AddFlatModifier(this.WeaponAttributes[q.AttributeType], this.Parent);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            base._HandleQuery(q);
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);

            return q;
        }

    }
}
