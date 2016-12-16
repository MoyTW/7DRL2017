using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public class Component_TracksTime : Component
    {
        private int lastActivationTick;
        private EntityAttributeType CooldownAttribute { get; }

        public Component_TracksTime(EntityAttributeType cooldownAttribute)
        {
            this.lastActivationTick = -9999;
            this.CooldownAttribute = cooldownAttribute;
        }

        public void RegisterActivated(int activationTick)
        {
            this.lastActivationTick = activationTick;
        }

        private void HandleQueryTicksToLive(GameQuery_TicksToLive q)
        {
            var cooldown = this.Parent.TryGetAttribute(this.CooldownAttribute).Value;
            var ticksToLive = this.lastActivationTick + cooldown - q.CurrentTick;
            q.RegisterTicksToLive(ticksToLive);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_TicksToLive)
                this.HandleQueryTicksToLive((GameQuery_TicksToLive)q);

            return q;
        }
    }
}
