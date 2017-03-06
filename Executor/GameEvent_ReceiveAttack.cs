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

        // Result info
        public bool ResultMissedDueToMissingBodyPart { get; private set; }
        public bool ResultDestroyedBodyPart { get; private set; }
        public int ResultDamage { get; private set; }

        public GameEvent_ReceiveAttack(GameEvent_PrepareAttack preparedAttack)
        {
            this.preparedAttack = preparedAttack;
        }

        public void RegisterAttackResults(bool hit, bool missedDueToMissingBodyPart = false, bool destroyedBodyPart = false,
            int damage = 0)
        {
            this.ResultMissedDueToMissingBodyPart = missedDueToMissingBodyPart;
            this.ResultDestroyedBodyPart = destroyedBodyPart;
            this.ResultDamage = damage;
            this.Completed = true;

            // TODO: Put this logging somewhere nicer!
            if (this.ResultMissedDueToMissingBodyPart)
            {
                Log.DebugLine(
                    String.Format("{0} ({1}) missed - the {2} of the target was already destroyed!",
                        this.Attacker, this.Weapon, this.SubTarget));
            }
            else if (this.ResultDestroyedBodyPart)
            {
                Log.DebugLine(String.Format("{0} ({1}) hit the {2}, destroying it!", this.Attacker,
                        this.Weapon, this.SubTarget));
            }
            else
            {
                Log.DebugLine(
                    String.Format("{0} ({1}) hit {2}!", this.Attacker, this.Weapon, this.SubTarget));
            }
        }
    }
}
