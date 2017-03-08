using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    public class CommandStub_Delay : CommandStub
    {
        public int APCost { get; }
        
        public CommandStub_Delay(string commandEID, int APCost) : base(commandEID)
        {
            this.APCost = APCost;
        }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            return new GameEvent_Delay(arena.CurrentTick, this.APCost, arena.ResolveEID(this.CommandEID));
        }

        public override string ToString()
        {
            return string.Format("Delay for {0} AP", this.APCost);
        }
    }

    class GameEvent_Delay : GameEvent_Command
    {
        public GameEvent_Delay(int commandTick, int APCost, Entity commandEntity)
            : base(commandTick, APCost, commandEntity)
        { }
    }
}
