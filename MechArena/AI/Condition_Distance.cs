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

        public override bool IsMet(GameQuery_Command commandQuery)
        {
            // TODO: This is gonna show up in a lot of conditions, and looks pretty janky.
            Entity target;
            if (commandQuery.CommandEntity == commandQuery.ArenaState.Mech1)
                target = commandQuery.ArenaState.Mech2;
            else
                target = commandQuery.ArenaState.Mech1;
            
            var targetPos = target.TryGetPosition();
            var selfPos = commandQuery.CommandEntity.TryGetPosition();
            int currDist = commandQuery.ArenaState.ArenaMap
                .GetCellsAlongLine(selfPos.X, selfPos.Y, targetPos.X, targetPos.Y)
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
