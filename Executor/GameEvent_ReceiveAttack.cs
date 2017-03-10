using RogueSharp;
using System;

namespace Executor
{
    class GameEvent_ReceiveAttack : GameEvent
    {
        private GameEvent_PrepareAttack preparedAttack;

        public Entity Attacker { get { return this.preparedAttack.CommandEntity; } }
        public Entity Weapon { get { return this.preparedAttack.ExecutorEntity; } }
        public Entity Target { get { return this.preparedAttack.Target; } }
        public BodyPartLocation SubTarget { get { return this.preparedAttack.SubTarget; } }
        public IMap GameMap { get; }

        public GameEvent_ReceiveAttack(GameEvent_PrepareAttack preparedAttack)
        {
            this.preparedAttack = preparedAttack;
        }

        public void RegisterAttackResults(string logMessage)
        {
            this.Completed = true;
            this.preparedAttack.RegisterResult(logMessage);
        }
    }
}
