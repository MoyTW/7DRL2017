using System;

namespace Executor.AI
{
    [Serializable()]
    class Action_Delay : AIAction
    {
        public override bool CanExecuteOn(GameQuery_Command commandQuery)
        {
            // Anything which takes actions must implicitly track time and be delayable, hence always true
            return true;
        }

        public override CommandStub GenerateCommand(GameQuery_Command commandQuery)
        {
            var remainingAP = commandQuery.CommandEntity.TryGetAttribute(EntityAttributeType.CURRENT_AP).Value;
            return new CommandStub_Delay(commandQuery.CommandEntity.EntityID, remainingAP);
        }
    }
}

