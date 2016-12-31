﻿using RogueSharp.Random;

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
        public const string WeaponTypeLabel = "Weapon";
        public static readonly List<BodyPartLocation> MechLocations = new List<BodyPartLocation> {
            BodyPartLocation.HEAD,
            BodyPartLocation.TORSO,
            BodyPartLocation.LEFT_ARM,
            BodyPartLocation.RIGHT_ARM,
            BodyPartLocation.LEFT_LEG,
            BodyPartLocation.RIGHT_LEG
        };

        #endregion

        #region Utilities

        private static Entity GetBodyPart(Entity mech, BodyPartLocation location)
        {
            return mech.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.BODY_PART))
                .SubEntities
                .Where(e => e.GetComponentOfType<Component_BodyPartLocation>().Location == location)
                .First();
        }

        private static void SlotAt(Entity mech, BodyPartLocation location, Entity slottable)
        {
            var bodyPart = GetBodyPart(mech, location);
            bodyPart.HandleEvent(new GameEvent_Slot(mech, bodyPart, slottable));
        }

        private static void FillLocationWith(Entity mech, BodyPartLocation location, Func<Entity> buildFn)
        {
            var container = GetBodyPart(mech, location).GetComponentOfType<Component_SlottedContainer>();
            var toAdd = buildFn();
            while (container.SlotsRemaining >= toAdd.GetComponentOfType<Component_Slottable>().SlotsRequired)
            {
                SlotAt(mech, location, toAdd);
                toAdd = buildFn();
            }
        }

        private static Entity BuildWeapon(string weaponLabel, int slotsRequired, int internalStructure,
            WeaponSize size, int toHit, int maxRange, int damage, int refireTicks)
        {
            return new Entity(label: weaponLabel, typeLabel: EntityBuilder.WeaponTypeLabel)
                .AddComponent(new Component_Slottable(slotsRequired))
                .AddComponent(new Component_InternalStructure(internalStructure))
                .AddComponent(new Component_Weapon(size, toHit, maxRange, damage, refireTicks));
        }

        private static Entity BuildAndSlotWeapon(Entity mech, BodyPartLocation location, string weaponLabel,
            int slotsRequired, int internalStructure, WeaponSize size, int toHit, int maxRange, int damage,
            int refireTicks)
        {
            var weapon = BuildWeapon(weaponLabel, slotsRequired, internalStructure, size, toHit, maxRange, damage,
                refireTicks);
            SlotAt(mech, location, weapon);
            return mech;
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

            BuildAndSlotWeapon(mech, BodyPartLocation.HEAD, "P.HL.", 1, 1, WeaponSize.SMALL, 9999, 10, 3, 25);
            BuildAndSlotWeapon(mech, BodyPartLocation.LEFT_ARM, "DM.CNNN.", 4, 32, WeaponSize.LARGE, 9999, 9999,
                9999, 1);
            BuildAndSlotWeapon(mech, BodyPartLocation.RIGHT_ARM, "DM.CNNN.", 4, 32, WeaponSize.LARGE, 9999, 9999,
                9999, 1);

            FillLocationWith(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator);
            FillLocationWith(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator);

            return mech;
        }

        public static Entity BuildArmoredMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);

            BuildAndSlotWeapon(mech, BodyPartLocation.HEAD, "AIIM.HL.", 1, 1, WeaponSize.SMALL, 5, 10, 1, 25);
            BuildAndSlotWeapon(mech, BodyPartLocation.LEFT_ARM, "AIIM.LAW.", 1, 1, WeaponSize.SMALL, 0, 10, 3, 25);
            BuildAndSlotWeapon(mech, BodyPartLocation.RIGHT_ARM, "AIIM.RAW.", 1, 1, WeaponSize.SMALL, 0, 10, 3, 25);

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildKnifeMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);

            BuildAndSlotWeapon(mech, BodyPartLocation.LEFT_ARM, "KNF.", 1, 2, WeaponSize.SMALL, 3, 1, 2, 13);
            BuildAndSlotWeapon(mech, BodyPartLocation.RIGHT_ARM, "KNF.", 1, 2, WeaponSize.SMALL, 3, 1, 2, 13);
            BuildAndSlotWeapon(mech, BodyPartLocation.LEFT_LEG, "KNF.", 1, 2, WeaponSize.SMALL, 3, 1, 2, 13);
            BuildAndSlotWeapon(mech, BodyPartLocation.RIGHT_LEG, "KNF.", 1, 2, WeaponSize.SMALL, 3, 1, 2, 13);

            FillLocationWith(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator);
            FillLocationWith(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator);

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildPaladinMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);

            BuildAndSlotWeapon(mech, BodyPartLocation.HEAD, "HD.LSR.", 1, 1, WeaponSize.SMALL, 5, 10, 1, 25);
            BuildAndSlotWeapon(mech, BodyPartLocation.LEFT_ARM, "SHLD.", 4, 20, WeaponSize.LARGE, 3, 1, 1, 50);
            BuildAndSlotWeapon(mech, BodyPartLocation.RIGHT_ARM, "SWRD.", 2, 4, WeaponSize.MEDIUM, 0, 1, 6, 20);

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildAlphaStrikerMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);

            Func<Entity> buildRocketPod = () => BuildWeapon("RCKT.PD.", 2, 2, WeaponSize.MEDIUM, -3, 6, 12, 200);

            FillLocationWith(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator);
            FillLocationWith(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator);

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, buildRocketPod);
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildRandomMech(string label, bool player, IRandom rand)
        {
            // DoomCannonMech is reserved for the Player
            var choice = rand.Next(3);
            switch (choice)
            {
                case 0:
                    return BuildArmoredMech(label, player);
                case 1:
                    return BuildKnifeMech(label, player);
                case 2:
                    return BuildPaladinMech(label, player);
                case 3:
                    return BuildAlphaStrikerMech(label, player);
                default:
                    return BuildNakedMech(label, player);
            }
        }

        #endregion
    }
}
