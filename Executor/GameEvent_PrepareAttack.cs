﻿using RogueSharp;
using System;
using System.Linq;

namespace Executor
{
    public class CommandStub_PrepareDirectionalAttack : CommandStub
    {
        public string AttackerEID { get; }
        public int x { get; }
        public int y { get; }
        public BodyPartLocation SubTarget { get; }

        public CommandStub_PrepareDirectionalAttack(string attackerEID, int x, int y, BodyPartLocation subTarget)
            : base (attackerEID)
        {
            this.AttackerEID = attackerEID;
            this.x = x;
            this.y = y;
            this.SubTarget = subTarget;
        }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            var equippedWeapon = arena.Player.GetComponentOfType<Component_Skeleton>()
                .InspectBodyPart(BodyPartLocation.RIGHT_ARM)
                .TryGetSubEntities(SubEntitiesSelector.WEAPON)
                .FirstOrDefault();

            // TODO: Don't require exact square
            var attackerEntity = arena.ResolveEID(this.AttackerEID);
            var attackerPosition = attackerEntity.TryGetPosition();
            var targetEntity = arena.EntityAtPosition(attackerPosition.X + this.x, attackerPosition.Y + this.y);
            if (targetEntity != null)
            {
                return new GameEvent_PrepareAttack(arena.CurrentTick, attackerEntity, targetEntity, equippedWeapon, 
                    arena.ArenaMap, this.SubTarget);
            }
            else
                return null;
        }
    }

    public class CommandStub_PrepareTargetedAttack : CommandStub
    {
        public string AttackerEID { get; }
        public string TargetLabel { get; }
        public string TargetEID { get; }
        public BodyPartLocation SubTarget { get; }

        public CommandStub_PrepareTargetedAttack(string attackerEID, string targetEID, string targetLabel,
            BodyPartLocation subTarget) : base(attackerEID)
        {
            this.AttackerEID = attackerEID;
            this.TargetLabel = targetLabel;
            this.TargetEID = targetEID;
            this.SubTarget = subTarget;
        }

        public override GameEvent_Command ReifyStub(ArenaState arena)
        {
            // TODO: Equipped items are *not* "Whatever is held in right right arm"
            var equippedWeapon = arena.Player.GetComponentOfType<Component_Skeleton>()
                    .InspectBodyPart(BodyPartLocation.RIGHT_ARM)
                    .TryGetSubEntities(SubEntitiesSelector.WEAPON)
                    .FirstOrDefault();

            return new GameEvent_PrepareAttack(arena.CurrentTick, arena.ResolveEID(this.AttackerEID),
                arena.ResolveEID(this.TargetEID), equippedWeapon, arena.ArenaMap, this.SubTarget);
        }

        public override string ToString()
        {
            return string.Format("Attack {0}'s {1}", this.TargetLabel, SubTarget);
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
