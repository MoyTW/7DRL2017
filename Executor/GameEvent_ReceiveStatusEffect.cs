using System;

namespace Executor
{
    public class CommandStub_ReceiveStatusEffect : CommandStub
    {
        public string TargetEID { get; }
        public string TargetLabel { get; }
        public string CasterEID { get; }
        public Component_StatusEffect StatusEffect { get; }

        public CommandStub_ReceiveStatusEffect(string targetEID, string targetLabel, string casterEID,
            Component_StatusEffect statusEffect)
            : base(targetEID)
        {
            this.TargetEID = targetEID;
            this.TargetLabel = targetLabel;
            this.CasterEID = casterEID;
            this.StatusEffect = statusEffect;
        }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            return new GameEvent_ReceiveStatusEffect(arena.CurrentTick, Config.ZERO, arena.ResolveEID(this.TargetEID),
                arena.ResolveEID(this.CasterEID), this.StatusEffect);
        }

        public override string ToString()
        {
            return string.Format("Apply {0} to {1}", this.StatusEffect, this.TargetLabel);
        }
    }

    public class GameEvent_ReceiveStatusEffect : GameEvent_Command
    {
        public Component_StatusEffect StatusEffect { get; }

        public override bool ShouldLog { get { return true; } }
        protected override string _LogMessage
        {
            get
            {
                return string.Format("{0} is now {1} (Source {2})", this.CommandEntity.Label,
                    this.StatusEffect.EffectLabel, this.ExecutorEntity.Label);
            }
        }

        public GameEvent_ReceiveStatusEffect(int commandTick, int APCost, Entity commandEntity, 
            Component_StatusEffect statusEffect) : this(commandTick, APCost, commandEntity, commandEntity, statusEffect)
        { }

        public GameEvent_ReceiveStatusEffect(int commandTick, int APCost, Entity commandEntity, Entity executorEntity,
            Component_StatusEffect statusEffect)
            : base(commandTick, APCost, commandEntity, executorEntity)
        {
            this.StatusEffect = statusEffect;
        }
    }
}

