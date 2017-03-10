using System;
using System.Collections.Immutable;

namespace Executor
{
    [Serializable()]
    public class StatusEffect_Blocking : StatusEffect
    {
        public int BlocksMax { get; }
        public int BlocksRemaining { get; private set; }
        public override string EffectLabel
        {
            get
            {
                return "Blocking";
            }
        }

        public StatusEffect_Blocking(int duration, int blocksMax) : base(duration)
        {
            this.BlocksMax = blocksMax;
            this.BlocksRemaining = blocksMax;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        protected override void _HandleEndTurn(GameEvent_EndTurn ev)
        {
            this.BlocksRemaining = this.BlocksMax;
        }

        private void HandleReceiveAttack(GameEvent_ReceiveAttack ev)
        {
            if (this.BlocksRemaining > 0)
            {
                this.BlocksRemaining--;
                ev.RegisterAttackResults(String.Format("BLOCK ({0} remaining)", this.BlocksRemaining));
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            base._HandleEvent(ev);
            if (ev is GameEvent_ReceiveAttack)
                this.HandleReceiveAttack((GameEvent_ReceiveAttack)ev);

            return ev;
        }
    }
}

