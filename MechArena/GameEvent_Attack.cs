using RogueSharp;
using RogueSharp.Random;

using System;

namespace Executor
{
    public class GameEvent_Attack : GameEvent_Command
    {
        // Attack info
        public int CurrentTick { get; }
        public Entity Target { get; }
        public BodyPartLocation SubTarget { get; private set; }
        public IMap GameMap { get; }
        public IRandom Rand { get; }

        // Result info
        public bool ResultHit { get; private set; }
        public bool ResultMissedDueToMissingBodyPart { get; private set; }
        public bool ResultDestroyedBodyPart { get; private set; }
        public int ResultDamage { get; private set; }

        public GameEvent_Attack(int currentTick, Entity attacker, Entity target, Entity weapon, IMap gameMap,
            IRandom rand, BodyPartLocation subTarget=BodyPartLocation.ANY) : base(attacker, weapon)
        {
            if (!weapon.HasComponentOfType<Component_Weapon>())
                throw new ArgumentException("Can't build attack event - weapon has no Weapon component!");

            this.CurrentTick = currentTick;
            this.Target = target;
            this.SubTarget = subTarget;
            this.GameMap = gameMap;
            this.Rand = rand;
        }

        public void RegisterRetargeting(BodyPartLocation newLocation)
        {
            this.SubTarget = newLocation;
        }

        public void RegisterAttackResults(bool hit, bool missedDueToMissingBodyPart=false, bool destroyedBodyPart=false,
            int damage=0)
        {
            this.ResultHit = hit;
            this.ResultMissedDueToMissingBodyPart = missedDueToMissingBodyPart;
            this.ResultDestroyedBodyPart = destroyedBodyPart;
            this.ResultDamage = damage;
            this.Completed = true;

            // TODO: Put this logging somewhere nicer!
            if (this.ResultMissedDueToMissingBodyPart)
            {
                Log.DebugLine(
                    String.Format("{0} ({1}) missed - the {2} of the target was already destroyed!",
                        this.ExecutorEntity, this.CommandEntity, this.SubTarget));
            }
            else if (!this.ResultHit)
                Log.DebugLine(String.Format("{0} missed {1}!", this.ExecutorEntity, this.CommandEntity));
            else if (this.ResultDestroyedBodyPart)
            {
                Log.DebugLine(String.Format("{0} ({1}) hit the {2}, destroying it!", this.ExecutorEntity, 
                        this.CommandEntity, this.SubTarget));
            }
            else if (this.ResultHit)
            {
                Log.DebugLine(
                    String.Format("{0} ({1}) hit {2}!", this.ExecutorEntity, this.CommandEntity, this.SubTarget));
            }
            else
            {
                throw new InvalidOperationException("This should never happen!");
            }
        }
    }
}
