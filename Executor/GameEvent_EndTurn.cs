using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    public class GameEvent_EndTurn : GameEvent
    {
        public int TurnTick { get; }
        public Entity CommandEntity { get; }

        public GameEvent_EndTurn(int turnTick, Entity commandEntity)
        {
            this.TurnTick = turnTick;
            this.CommandEntity = commandEntity;
        }
    }
}
