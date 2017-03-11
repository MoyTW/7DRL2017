using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor.AI
{
    enum DistanceOption
    {
        MELEE_RANGE,
        // TODO: Implement the following!
        MY_MEDIUM_RANGE,
        MY_LONGEST_RANGE,
        ENEMY_LONGEST_RANGE
        // MY_OPTIMAL_RANGE,
        // ENEMY_OPTIMAL_RANGE
    }

    enum ComparisonOperator
    {
        EQUAL,
        LESS_THAN,
        LESS_THAN_EQUAL,
        GREATER_THAN,
        GREATER_THAN_EQUAL
    }

    [Serializable()]
    class Condition_Distance : Condition
    {
        public DistanceOption Option { get; }
        public ComparisonOperator Operator { get; }

        public Condition_Distance() { }

        public Condition_Distance(ComparisonOperator op, DistanceOption option)
        {
            this.Operator = op;
            this.Option = option;
        }

        private int? ResolveOptionDistance(GameQuery_Command commandQuery, Entity target)
        {
            switch (this.Option)
            {
                case DistanceOption.MELEE_RANGE:
                    return 1;
                case DistanceOption.MY_LONGEST_RANGE:
                    return commandQuery.CommandEntity.TryGetSubEntities(SubEntitiesSelector.WEAPON)
                        .Select(e => e.TryGetAttribute(EntityAttributeType.MAX_RANGE, e).Value)
                        .Max();
                case DistanceOption.MY_MEDIUM_RANGE:
                    return commandQuery.CommandEntity.TryGetSubEntities(SubEntitiesSelector.WEAPON)
                        .Select(e => e.TryGetAttribute(EntityAttributeType.MAX_RANGE, e).Value)
                        .Max() / 2;
                case DistanceOption.ENEMY_LONGEST_RANGE:
                    return target.TryGetSubEntities(SubEntitiesSelector.WEAPON)
                        .Select(e => e.TryGetAttribute(EntityAttributeType.MAX_RANGE, e).Value)
                        .Max();
                default:
                    throw new NotImplementedException();
            }
        }

        public override bool IsMet(GameQuery_Command commandQuery)
        {
            // TODO: This is gonna show up in a lot of conditions, and looks pretty janky.
            Entity target = commandQuery.ArenaState.Player;

            // TODO: Smooth out this int? business!
            // It's an int? because some conditions (asking for WEAPON_RANGE on something which has no range, like a
            // MechSkeleton, but which does get turns) are nonsensical.
            int? optionDistance = this.ResolveOptionDistance(commandQuery, target);
            if (optionDistance == null)
                return true;

            int currDist = ArenaState.DistanceBetweenEntities(target, commandQuery.CommandEntity);

            switch (this.Operator)
            {
                case ComparisonOperator.EQUAL:
                    return currDist == optionDistance;
                case ComparisonOperator.LESS_THAN:
                    return currDist < optionDistance;
                case ComparisonOperator.LESS_THAN_EQUAL:
                    return currDist <= optionDistance;
                case ComparisonOperator.GREATER_THAN:
                    return currDist > optionDistance;
                case ComparisonOperator.GREATER_THAN_EQUAL:
                    return currDist >= optionDistance;
                default:
                    throw new InvalidOperationException("Condition_Distance can't handle " + this.Operator);
            }
        }

        public override string ToString()
        {
            return string.Format("[Condition_Distance: Option={0}, Operator={1}]", Option, Operator);
        }
    }
}
