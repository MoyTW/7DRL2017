using System;
using System.Collections.Generic;

namespace MechArena
{
    [Serializable()]
    public class Component_Weapon : Component_TracksTime
    {
        public MountSize Size { get; }
        private readonly Dictionary<EntityAttributeType, int> WeaponAttributes;

        // TODO: I would like to define a construct which can be read from a file to construct these!
        // Right now I'll just hard-code them all and feel bad about it.
        public Component_Weapon(MountSize size, int toHit, int maxRange, int damage, int refireTicks)
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

        protected override ISet<SubEntitiesSelector> _MatchingSelectors()
        {
            var baseMatching = base._MatchingSelectors();
            baseMatching.Add(SubEntitiesSelector.WEAPON);
            return baseMatching;
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (this.Parent == q.BaseEntity && this.WeaponAttributes.ContainsKey(q.AttributeType))
                q.RegisterBaseValue(this.WeaponAttributes[q.AttributeType]);
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
