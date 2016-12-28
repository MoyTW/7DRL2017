using RogueSharp;
using RogueSharp.Random;

using System;

namespace MechArena
{
    class GameEvent_Attack : GameEvent_Command
    {
        public int CurrentTick { get; }
        public Entity Target { get; }
        public BodyPartLocation SubTarget { get; set; }
        public IMap GameMap { get; }
        public IRandom Rand { get; }

        private GameQuery_EntityAttribute attackerToHit, attackerDamage;

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

        public void RegisterAttackerAttributes(GameQuery_EntityAttribute toHit, GameQuery_EntityAttribute damage)
        {
            if (!(toHit.AttributeType == EntityAttributeType.TO_HIT))
                throw new ArgumentException("Can't register toHit for attack, type != TO_HIT");
            if (!(damage.AttributeType == EntityAttributeType.DAMAGE))
                throw new ArgumentException("Can't register damage for attack, type != DAMAGE");

            this.attackerToHit = toHit;
            this.attackerDamage = damage;
        }
    }
}
