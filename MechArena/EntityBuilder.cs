using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public static class EntityBuilder
    {
        // TODO: Hardcoded values all over!
        public const string PartTypeLabel = "Part";
        public const string MechTypeLabel = "Mech";

        public static Entity BuildBodyPart(BodyPartLocation location, int slotSpace, int internalStructure)
        {
            return new Entity(label: location.ToString(), typeLabel: "BodyPart")
                .AddComponent(new Component_BodyPartLocation(location))
                .AddComponent(new Component_SlottedContainer(slotSpace))
                .AddComponent(new Component_InternalStructure(internalStructure));
        }

        public static Entity BuildNakedMech(string label)
        {
            return new Entity(label: label, typeLabel: MechTypeLabel)
                .AddComponent(new Component_MechSkeleton());
        }
        
        public static Entity BuildArmorPart()
        {
            return new Entity(label: "Armor", typeLabel: PartTypeLabel)
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(4));
        }

        public static Entity BuildPlayer()
        {
            // MechSkeletons should always have Attacker?
            var player = new Entity(label: "PlayerMech", typeLabel: MechTypeLabel)
                .AddComponent(new Component_MechSkeleton())
                .AddComponent(new Component_Attacker());
            var bodyParts = player.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART));
            foreach(var part in bodyParts.SubEntities)
            {
                // Normally you should not be able to directly slot weapons - they should be mounted/holstered!
                var weapon = new Entity(label: "TestWeapon", typeLabel: "Weapon")
                    .AddComponent(new Component_Slottable(1))
                    .AddComponent(new Component_InternalStructure(1))
                    .AddComponent(new Component_Weapon(WeaponSize.SMALL, 0, 10, 3, 25));
                part.HandleEvent(new GameEvent_Slot(weapon, part));
            }

            return player;
        }

        public static Entity BuildArmoredMech(string label)
        {
            // MechSkeletons should always have Attacker?
            var mech = new Entity(label: "PlayerMech", typeLabel: MechTypeLabel)
                .AddComponent(new Component_MechSkeleton())
                .AddComponent(new Component_Attacker());
            var bodyParts = mech.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART));
            foreach (var part in bodyParts.SubEntities)
            {
                var container = part.GetComponentOfType<Component_SlottedContainer>();
                while (container.SlotsRemaining > 0)
                {
                    var armor = BuildArmorPart();
                    part.HandleEvent(new GameEvent_Slot(armor, part));
                }
            }

            return mech;
        }
    }
}
