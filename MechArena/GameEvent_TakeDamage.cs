using RogueSharp.Random;

using System;

namespace MechArena
{
    public class GameEvent_TakeDamage : GameEvent
    {
        private int damageRemaining;

        public int TotalDamage { get; }
        public int DamageRemaining { get { return this.damageRemaining; } }
        public IRandom Rand { get; }

        public GameEvent_TakeDamage(int totalDamage, IRandom rand)
        {
            this.TotalDamage = totalDamage;
            this.damageRemaining = totalDamage;
            this.Rand = rand;
        }

        public void Notify_DamageTaken(int applied)
        {
            if (applied > this.damageRemaining)
                throw new ArgumentException("Applied more damage than the event indicated!");

            this.damageRemaining -= applied;
            if (this.damageRemaining == 0)
                this.Completed = true;
        }
    }
}
