using RogueSharp.Random;

using System;

namespace Executor
{
    public class GameEvent_TakeDamage : GameEvent
    {
        private int damageRemaining;

        public int TotalDamage { get; }
        public int DamageRemaining { get { return this.damageRemaining; } }

        public GameEvent_TakeDamage(int totalDamage)
        {
            this.TotalDamage = totalDamage;
            this.damageRemaining = totalDamage;
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
