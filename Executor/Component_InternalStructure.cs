using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public class Component_InternalStructure : Component
    {
        private int structureDestroyed;

        public int StructureMax { get; }
        public int StructureDestroyed { get { return this.structureDestroyed; } }
        public int StructureRemaining { get { return this.StructureMax - this.StructureDestroyed; } }
        public bool Destroyed { get { return this.StructureRemaining == 0; } }

        public Component_InternalStructure(int structureMax)
        {
            this.StructureMax = structureMax;
            this.structureDestroyed = 0;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleTakeDamage(GameEvent_TakeDamage ev)
        {
            if (ev.DamageRemaining >= this.StructureRemaining)
            {
                ev.Notify_DamageTaken(this.StructureRemaining);
                this.structureDestroyed += this.StructureRemaining;
            }
            else
            {
                int damageToTake = ev.DamageRemaining;
                this.structureDestroyed += damageToTake;
                ev.Notify_DamageTaken(damageToTake);
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_TakeDamage)
                this.HandleTakeDamage((GameEvent_TakeDamage)ev);

            return ev;
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (q.AttributeType == EntityAttributeType.STRUCTURE)
            {
                if (q.BaseEntity == this.Parent)
                    q.RegisterBaseValue(this.StructureRemaining);
                else
                    q.AddFlatModifier(this.StructureRemaining, this.Parent);
            }
        }

        private void HandleQueryDestroyed(GameQuery_Destroyed q)
        {
            if (this.Destroyed)
                q.RegisterDestroyed();
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);
            if (q is GameQuery_Destroyed)
                this.HandleQueryDestroyed((GameQuery_Destroyed)q);

            return q;
        }
    }
}
