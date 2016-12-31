using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public static class EntityBuilder
    {
        #region Values

        // TODO: Hardcoded values all over!
        public const string SlottablePartTypeLabel = "Slottable Part";
        public const string BodyPartTypeLabel = "Body Part";
        public const string MechTypeLabel = "Mech";

        #endregion

        #region Utilities

        private static void SlotAt(Entity mech, BodyPartLocation location, Entity slottable)
        {
            var bodyPart = GetBodyPart(location,
                mech.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART)).SubEntities);
            bodyPart.HandleEvent(new GameEvent_Slot(mech, bodyPart, slottable));
        }

        #endregion

        #region Slottable Parts

        public static Entity BuildAccelerator()
        {
            return new Entity(label: "Accel.", typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_AttributeModifierMult(EntityAttributeType.SPEED, .9))
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(1));
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

        public static Entity BuildArmorPart()
        {
            return new Entity(label: "Armor", typeLabel: BodyPartTypeLabel)
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(4));
        }

        #endregion

        #region Mechs

        #region Mech Base

        public static Entity BuildBodyPart(BodyPartLocation location, int slotSpace, int internalStructure)
        {
            return new Entity(label: location.ToString(), typeLabel: "BodyPart")
                .AddComponent(new Component_BodyPartLocation(location))
                .AddComponent(new Component_SlottedContainer(slotSpace))
                .AddComponent(new Component_SlottedStructure())
                .AddComponent(new Component_InternalStructure(internalStructure));
        }

        public static Entity BuildNakedMech(string label, bool player)
        {
            var mech = new Entity(label: label, typeLabel: MechTypeLabel)
                .AddComponent(new Component_MechSkeleton())
                .AddComponent(new Component_Attacker());

            if (player)
                mech.AddComponent(new Component_Player());
            else
                mech.AddComponent(new Component_AI());

            SlotAt(mech, BodyPartLocation.HEAD, BuildSensorPackage());
            SlotAt(mech, BodyPartLocation.TORSO, BuildPowerPlant());
            SlotAt(mech, BodyPartLocation.LEFT_ARM, BuildArmActuator());
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, BuildArmActuator());
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildArmActuator());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildArmActuator());

            // Slot in all the required components
            return mech;
        }

        #endregion

        public static Entity BuildDoomCannonMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);

            var weapon = new Entity(label: "P.HL", typeLabel: "Weapon")
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(1))
                .AddComponent(new Component_Weapon(WeaponSize.SMALL, 9999, 10, 3, 25));
            SlotAt(mech, BodyPartLocation.HEAD, weapon);

            var largeWeaponMount = new Entity(label: "L.Wpn.Mnt.", typeLabel: "Weapon")
                .AddComponent(new Component_Slottable(8))
                .AddComponent(new Component_InternalStructure(8));
            SlotAt(mech, BodyPartLocation.TORSO, largeWeaponMount);

            var doomCannon = new Entity(label: "DM.CNNN.", typeLabel: "Weapon")
                .AddComponent(new Component_Slottable(4))
                .AddComponent(new Component_InternalStructure(32))
                .AddComponent(new Component_Weapon(WeaponSize.LARGE, 9999, 9999, 9999, 1));
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, doomCannon);

            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator());

            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator());

            return mech;
        }

        public static Entity BuildArmoredMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);
            var bodyParts = mech.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART)).SubEntities;

            var weapon = new Entity(label: "AIIM.HL", typeLabel: "Weapon")
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(1))
                .AddComponent(new Component_Weapon(WeaponSize.SMALL, 9999, 10, 1, 25));
            SlotAt(mech, BodyPartLocation.HEAD, weapon);

            var leftArmWeapon = new Entity(label: "AIIM.LAW", typeLabel: "Weapon")
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(1))
                .AddComponent(new Component_Weapon(WeaponSize.SMALL, 9999, 10, 3, 25));
            SlotAt(mech, BodyPartLocation.LEFT_ARM, leftArmWeapon);

            var rightArmWeapon = new Entity(label: "AIIM.RAW", typeLabel: "Weapon")
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(1))
                .AddComponent(new Component_Weapon(WeaponSize.SMALL, 9999, 10, 3, 25));
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, rightArmWeapon);

            foreach (var part in bodyParts)
            {
                var container = part.GetComponentOfType<Component_SlottedContainer>();
                while (container.SlotsRemaining > 0)
                {
                    var armor = BuildArmorPart();
                    part.HandleEvent(new GameEvent_Slot(mech, part, armor));
                }
            }

            return mech;
        }

        #endregion
    }
}
