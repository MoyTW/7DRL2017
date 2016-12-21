using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public static class EntityBuilder
    {
        // TODO: Hardcoded values all over!
        public const string SlottablePartTypeLabel = "Slottable Part";
        public const string BodyPartTypeLabel = "Body Part";
        public const string MechTypeLabel = "Mech";

        public static Entity BuildBodyPart(BodyPartLocation location, int slotSpace, int internalStructure)
        {
            return new Entity(label: location.ToString(), typeLabel: "BodyPart")
                .AddComponent(new Component_BodyPartLocation(location))
                .AddComponent(new Component_SlottedContainer(slotSpace))
                .AddComponent(new Component_SlottedStructure())
                .AddComponent(new Component_InternalStructure(internalStructure));
        }

        public static Entity BuildPowerPlant()
        {
            return new Entity(label: "Pwr.Plnt.", typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_Slottable(5))
                .AddComponent(new Component_InternalStructure(5));
        }

        public static Entity BuildSensorPackage()
        {
            return new Entity(label: "Snsr.Pckg.", typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_Slottable(2))
                .AddComponent(new Component_InternalStructure(2));
        }

        public static Entity BuildArmActuator()
        {
            return new Entity(label: "Arm.Actr.", typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_Slottable(2))
                .AddComponent(new Component_InternalStructure(2));
        }

        public static Entity BuildLegActuator()
        {
            return new Entity(label: "Leg.Actr.", typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_Slottable(3))
                .AddComponent(new Component_InternalStructure(3));
        }

        private static Entity GetBodyPart(BodyPartLocation location, IEnumerable<Entity> entities)
        {
            return entities.Where(e => e.GetComponentOfType<Component_BodyPartLocation>().Location == location)
                .First();
        }

        public static Entity BuildNakedMech(string label)
        {
            var mech = new Entity(label: label, typeLabel: MechTypeLabel)
                .AddComponent(new Component_MechSkeleton())
                .AddComponent(new Component_Attacker());

            var bodyParts = mech.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART)).SubEntities;
            GetBodyPart(BodyPartLocation.HEAD, bodyParts).HandleEvent(
                new GameEvent_Slot(BuildSensorPackage(), GetBodyPart(BodyPartLocation.HEAD, bodyParts)));
            GetBodyPart(BodyPartLocation.TORSO, bodyParts).HandleEvent(
                new GameEvent_Slot(BuildPowerPlant(), GetBodyPart(BodyPartLocation.TORSO, bodyParts)));
            GetBodyPart(BodyPartLocation.LEFT_ARM, bodyParts).HandleEvent(
                new GameEvent_Slot(BuildArmActuator(), GetBodyPart(BodyPartLocation.LEFT_ARM, bodyParts)));
            GetBodyPart(BodyPartLocation.RIGHT_ARM, bodyParts).HandleEvent(
                new GameEvent_Slot(BuildArmActuator(), GetBodyPart(BodyPartLocation.RIGHT_ARM, bodyParts)));
            GetBodyPart(BodyPartLocation.LEFT_LEG, bodyParts).HandleEvent(
                new GameEvent_Slot(BuildLegActuator(), GetBodyPart(BodyPartLocation.LEFT_LEG, bodyParts)));
            GetBodyPart(BodyPartLocation.RIGHT_LEG, bodyParts).HandleEvent(
                new GameEvent_Slot(BuildLegActuator(), GetBodyPart(BodyPartLocation.RIGHT_LEG, bodyParts)));

            // Slot in all the required components
            return mech;
        }
        
        public static Entity BuildArmorPart()
        {
            return new Entity(label: "Armor", typeLabel: BodyPartTypeLabel)
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(4));
        }

        public static Entity BuildPlayer()
        {
            var player = BuildNakedMech("Player Mech");
            var bodyParts = player.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART));

            var weapon = new Entity(label: "Headlight", typeLabel: "Weapon")
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(1))
                .AddComponent(new Component_Weapon(WeaponSize.SMALL, 9999, 10, 9, 25));
            var head = bodyParts.SubEntities[0];
            head.HandleEvent(new GameEvent_Slot(weapon, head));

            var largeWeaponMount = new Entity(label: "L.Wpn.Mnt.", typeLabel: "Weapon")
                .AddComponent(new Component_Slottable(8))
                .AddComponent(new Component_InternalStructure(8));
            var torso = bodyParts.SubEntities[1];
            torso.HandleEvent(new GameEvent_Slot(largeWeaponMount, torso));


            /*
            foreach(var part in bodyParts.SubEntities)
            {
                // Normally you should not be able to directly slot weapons - they should be mounted/holstered!
                var weapon = new Entity(label: "TestWeapon", typeLabel: "Weapon")
                    .AddComponent(new Component_Slottable(1))
                    .AddComponent(new Component_InternalStructure(1))
                    .AddComponent(new Component_Weapon(WeaponSize.SMALL, 0, 10, 3, 25));
                part.HandleEvent(new GameEvent_Slot(weapon, part));
            }
            */

            return player;
        }

        public static Entity BuildArmoredMech(string label)
        {
            var mech = BuildNakedMech("Armored Mech").AddComponent(new Component_Attacker());

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
