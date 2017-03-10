using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.AI
{
    [Serializable()]
    class Condition_CanSeeEnemy : Condition
    {
        public override bool IsMet(GameQuery_Command commandQuery)
        {
            // TODO: This is gonna show up in a lot of conditions, and looks pretty janky.
            Entity target = commandQuery.ArenaState.Player;

            var targetPos = target.TryGetPosition();
            var selfPos = commandQuery.CommandEntity.TryGetPosition();
            // TODO: I don't really subscribe to Demeter, but this is a pretty long chain!
            var path = commandQuery.ArenaState.ArenaMap
                .GetCellsAlongLine(selfPos.X, selfPos.Y, targetPos.X, targetPos.Y);

            return !path.Any(c => !c.IsWalkable);
        }
    }
}
