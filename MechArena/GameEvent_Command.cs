using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public class GameEvent_Command : GameEvent
    {
        public Entity MapEntity { get; }
        public Entity ActionEntity { get; }
        public GameEvent Action { get; }

        public GameEvent_Command(Entity mapEntity, Entity actionEntity, GameEvent action)
        {
            this.MapEntity = mapEntity;
            this.ActionEntity = actionEntity;
            this.Action = action;
        }
    }
}
