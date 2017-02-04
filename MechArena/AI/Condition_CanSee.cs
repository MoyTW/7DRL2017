using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI
{
    class Condition_CanSeeEnemy : Condition
    {
        public override bool IsMet(Entity self, ArenaState state)
        {
            // TODO: This is gonna show up in a lot of conditions, and looks pretty janky.
            var enemy = state.Mech1 == self ? state.Mech2 : state.Mech1;

            var enemyPos = enemy.TryGetPosition();
            var attackerPos = self.TryGetPosition();
            var path = state.ArenaMap.GetCellsAlongLine(attackerPos.X, attackerPos.Y, enemyPos.X, enemyPos.Y);

            return !path.Any(c => !c.IsWalkable);
        }
    }
}
