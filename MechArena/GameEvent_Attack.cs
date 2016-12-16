using RogueSharp;

using System;

namespace MechArena
{
    class GameEvent_Attack : GameEvent
    {
        public int CurrentTick { get; }
        public Entity Attacker { get; }
        public Entity Target { get; }
        public BodyPartLocation SubTarget { get; set; }
        public Entity Weapon { get; }
        public IMap GameMap { get; }

        private GameQuery_EntityAttribute attackerToHit, attackerDamage;

        public GameEvent_Attack(int currentTick, Entity attacker, Entity target, Entity weapon, IMap gameMap,
            BodyPartLocation subTarget=BodyPartLocation.ANY)
        {
            this.CurrentTick = currentTick;
            this.Attacker = attacker;
            this.Target = target;
            this.SubTarget = subTarget;
            this.Weapon = weapon;
            this.GameMap = gameMap;
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
