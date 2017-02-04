using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI
{
    enum ComparisonOperator
    {
        EQUAL,
        LESS_THAN,
        GREATER_THAN
    }

    class Condition_Distance : Condition
    {
        public int Distance { get; }
        public ComparisonOperator Operator { get; }

        public override bool IsMet(Entity self, ArenaState state)
        {
            // TODO: This is gonna show up in a lot of conditions, and looks pretty janky.
            var enemy = state.Mech1 == self ? state.Mech2 : state.Mech1;
            
            var enemyPos = enemy.TryGetPosition();
            var attackerPos = self.TryGetPosition();
            int currDist = state.ArenaMap.GetCellsAlongLine(attackerPos.X, attackerPos.Y, enemyPos.X, enemyPos.Y)
                .Count();

            switch(this.Operator)
            {
                case ComparisonOperator.EQUAL:
                    return currDist == this.Distance;
                case ComparisonOperator.LESS_THAN:
                    return currDist < this.Distance;
                case ComparisonOperator.GREATER_THAN:
                    return currDist > this.Distance;
                default:
                    throw new InvalidOperationException("Condition_Distance can't handle " + this.Operator);
            }
        }
    }
}
