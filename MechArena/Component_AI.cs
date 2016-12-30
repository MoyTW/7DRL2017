﻿using System;
using System.Linq;

namespace MechArena
{
    [Serializable()]
    class Component_AI : Component
    {
        private void HandleQueryCommand(GameQuery_Command q)
        {
            Entity target;
            if (q.CommandEntity == q.ArenaState.Mech1)
                target = q.ArenaState.Mech2;
            else
                target = q.ArenaState.Mech1;

            if (q.ExecutorEntity.HasComponentOfType<Component_MechSkeleton>())
            {
                var targetPos = target.TryGetPosition();
                var attackerPos = q.CommandEntity.TryGetPosition();
                var nextCell = q.ArenaState.ArenaMap
                    .GetCellsAlongLine(attackerPos.X, attackerPos.Y, targetPos.X, targetPos.Y)
                    .Take(2)
                    .Last();
                var moveCommand = new GameEvent_MoveSingle(q.CommandEntity, q.ArenaState.CurrentTick,
                    nextCell.X - attackerPos.X, nextCell.Y - attackerPos.Y, q.ArenaState);
                q.RegisterCommand(moveCommand);
            }
            else if (q.ExecutorEntity.HasComponentOfType<Component_Weapon>())
            {
                // TODO: Ability to query for LOS/Distance?
                var attackCommand = new GameEvent_Attack(q.ArenaState.CurrentTick, q.CommandEntity, target,
                    q.ExecutorEntity, q.ArenaState.ArenaMap, q.Rand);
                q.RegisterCommand(attackCommand);
            }
            else
            {
                throw new ArgumentException("Entity " + q.ExecutorEntity + " is not Mech, Weapon! Cannot command!");
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Command)
                this.HandleQueryCommand((GameQuery_Command)q);

            return q;

        }
    }
}
