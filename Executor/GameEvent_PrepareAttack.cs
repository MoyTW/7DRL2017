using RogueSharp;
using System;

namespace Executor
{
    public class GameEvent_PrepareAttack : GameEvent_Command
    {
        // Attack info
        public Entity Target { get; }
        public BodyPartLocation SubTarget { get; private set; }
        public IMap GameMap { get; }

        public GameEvent_PrepareAttack(int commandTick, Entity attacker, Entity target, Entity weapon, IMap gameMap,
            BodyPartLocation subTarget) : base(commandTick, Config.ONE, attacker, weapon)
        {
            if (!weapon.HasComponentOfType<Component_Weapon>())
                throw new ArgumentException("Can't build attack event - weapon has no Weapon component!");

            this.Target = target;
            this.SubTarget = subTarget;
            this.GameMap = gameMap;
        }
    }
}
