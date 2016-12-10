using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class Component_InternalStructure : Component
    {
        private int structureDestroyed;

        public int StructureMax { get; }
        public int StructureDestroyed { get { return this.structureDestroyed; } }
        public int StructureRemaining { get { return this.StructureMax - this.StructureDestroyed; } }

        public Component_InternalStructure(int structureMax)
        {
            this.StructureMax = structureMax;
            this.structureDestroyed = 0;
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
    }
}
