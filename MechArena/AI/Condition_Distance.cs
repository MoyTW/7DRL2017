using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.AI
{
    enum DistanceOption
    {
        THIS_WEAPON_RANGE
        // TODO: Implement the following!
        // MY_LONGEST_RANGE,
        // ENEMY_LONGEST_RANGE,
        // MY_OPTIMAL_RANGE,
        // ENEMY_OPTIMAL_RANGE
    }

    enum ComparisonOperator
    {
        EQUAL,
        LESS_THAN,
        GREATER_THAN
    }

    [Serializable()]
    class Condition_Distance : Condition
    {
        public DistanceOption Option { get; }
        public ComparisonOperator Operator { get; }

        public Condition_Distance(ComparisonOperator op, DistanceOption option)
        {
            this.Operator = op;
            this.Option = option;
        }

        private int ResolveOptionDistance(GameQuery_Command commandQuery, Entity target)
        {
            switch (this.Option)
            {
                case DistanceOption.THIS_WEAPON_RANGE:
                    var range = commandQuery.ExecutorEntity
                        .TryGetAttribute(EntityAttributeType.MAX_RANGE, commandQuery.ExecutorEntity);
                    return range.Value;
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool IsMet(GameQuery_Command commandQuery)
        {
            // TODO: This is gonna show up in a lot of conditions, and looks pretty janky.
            Entity target;
            if (commandQuery.CommandEntity == commandQuery.ArenaState.Mech1)
                target = commandQuery.ArenaState.Mech2;
            else
                target = commandQuery.ArenaState.Mech1;
            
            int optionDistance = this.ResolveOptionDistance(commandQuery, target);

            var targetPos = target.TryGetPosition();
            var selfPos = commandQuery.CommandEntity.TryGetPosition();
            int currDist = commandQuery.ArenaState.ArenaMap
                .GetCellsAlongLine(selfPos.X, selfPos.Y, targetPos.X, targetPos.Y)
                .Count();

            switch(this.Operator)
            {
                case ComparisonOperator.EQUAL:
                    return currDist == optionDistance;
                case ComparisonOperator.LESS_THAN:
                    return currDist < optionDistance;
                case ComparisonOperator.GREATER_THAN:
                    return currDist > optionDistance;
                default:
                    throw new InvalidOperationException("Condition_Distance can't handle " + this.Operator);
            }
        }
    }
}
