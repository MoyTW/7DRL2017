using RogueSharp;
using System;
using System.Linq;

namespace Executor
{
    public class CommandStub_PrepareAttack : CommandStub
    {
        public Entity Attacker { get; }
        public Entity Target { get; }
        public BodyPartLocation SubTarget { get; }

        public CommandStub_PrepareAttack(Entity attacker, Entity target, BodyPartLocation subTarget) : base(attacker)
        {
            this.Attacker = attacker;
            this.Target = target;
            this.SubTarget = subTarget;
        }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            // TODO: Equipped items are *not* "Whatever is held in right right arm"
            var equippedWeapon = arena.Player.GetComponentOfType<Component_Skeleton>()
                    .InspectBodyPart(BodyPartLocation.RIGHT_ARM)
                    .TryGetSubEntities(SubEntitiesSelector.WEAPON)
                    .FirstOrDefault();

            return new GameEvent_PrepareAttack(arena.CurrentTick, this.Attacker, this.Target, equippedWeapon,
                arena.ArenaMap, this.SubTarget);
        }
    }

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
