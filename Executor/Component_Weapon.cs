using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public class Component_Weapon : Component_TracksTime
    {
        public AttachmentSize Size { get; }
        public readonly Dictionary<EntityAttributeType, int> WeaponAttributes;

        // TODO: I would like to define a construct which can be read from a file to construct these!
        // Right now I'll just hard-code them all and feel bad about it.
        public Component_Weapon(AttachmentSize size, int toHit, int maxRange, int damage, int refireTicks)
            : base(EntityAttributeType.REFIRE_TICKS)
        {
            this.Size = size;
            this.WeaponAttributes = new Dictionary<EntityAttributeType, int>() {
                { EntityAttributeType.MAX_RANGE, maxRange },
                { EntityAttributeType.DAMAGE, damage },
                { EntityAttributeType.REFIRE_TICKS, refireTicks },
            };
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return base._MatchingSelectors().Add(SubEntitiesSelector.WEAPON);
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
