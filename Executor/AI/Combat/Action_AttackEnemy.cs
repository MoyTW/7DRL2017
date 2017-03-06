using System;

namespace Executor.AI.Combat
{
    [Serializable()]
    class Action_AttackEnemy : AIAction
    {
        public override bool CanExecuteOn(GameQuery_Command commandQuery)
        {
            throw new NotImplementedException();
        }

        public override GameEvent_Command GenerateCommand(GameQuery_Command commandQuery)
        {
            Entity target;
            if (commandQuery.CommandEntity == commandQuery.ArenaState.Player)
                target = commandQuery.ArenaState.Mech2;
            else
                target = commandQuery.ArenaState.Player;

            throw new NotImplementedException();
            /*
            return new GameEvent_PrepareAttack(commandQuery.ArenaState.CurrentTick, commandQuery.CommandEntity, target,
                commandQuery.ExecutorEntity, commandQuery.ArenaState.ArenaMap, BodyPartLocation.TORSO);
            */
        }

        public override System.Collections.Generic.IEnumerable<SingleClause> EnumerateClauses()
        {
            yield return new Action_AttackEnemy();
        }
    }
}

