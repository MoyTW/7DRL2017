﻿using System;
using System.Collections.Immutable;

namespace Executor
{
    public class Component_BlockBullets : Component_StatusEffect
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

        public Component_BlockBullets(int duration, int blocksMax) : base(duration)
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
                ev.RegisterAttackResults(false);
                this.BlocksRemaining--;
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
