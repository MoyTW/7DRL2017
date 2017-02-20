using RogueSharp;
using RogueSharp.Random;

using System;

namespace MechArena
{
    public class GameEvent_Attack : GameEvent_Command
    {
        public int CurrentTick { get; }
        public Entity Target { get; }
        public BodyPartLocation SubTarget { get; set; }
        public IMap GameMap { get; }
        public IRandom Rand { get; }

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
    }
}
