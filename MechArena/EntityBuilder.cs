using RogueSharp.Random;

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
        public const string ArmActuatorLabel = "Arm.Actr.";
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

        // Exposing this in this manner is absolutely a hack!
        public static readonly List<Func<Entity>> BuildPartFunctions = new List<Func<Entity>>()
        {
            BuildMountedMissile,
            BuildMountedMiniMissile,
            BuildMountedSniperRifle,
            BuildMountedRifle,
            BuildMountedMachinegun,
            BuildMountedShotgun,
            BuildMountedPistol,
            BuildMountedRockets,
            BuildMountedDagger,
            BuildMountedSword,
            BuildMountedHammer,
            BuildAccelerator,
            BuildSensorPackage,
            BuildArmorPart
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

        public static void SlotAt(Entity mech, BodyPartLocation location, Entity slottable)
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

        private static Entity BuildWeapon(string weaponLabel, MountSize size, int toHit, int maxRange, int damage, int refireTicks)
        {
            return new Entity(label: weaponLabel, typeLabel: EntityBuilder.WeaponTypeLabel)
                .AddComponent(new Component_Mountable(size))
                .AddComponent(new Component_Weapon(size, toHit, maxRange, damage, refireTicks));
        }

        private static Entity BuildMountForWeapon(Entity weapon)
        {
            var mount = BuildMount(weapon.GetComponentOfType<Component_Mountable>().SizeRequired);
            mount.HandleEvent(new GameEvent_Slot(null, mount, weapon));
            return mount;
        }

        private static Entity BuildHolsterForWeapon(Entity weapon)
        {
            var holster = BuildHolster(weapon.GetComponentOfType<Component_Mountable>().SizeRequired);
            holster.HandleEvent(new GameEvent_Slot(null, holster, weapon));
            return holster;
        }

        private static Entity MountOntoArm(Entity mech, BodyPartLocation location, Entity mountable)
        {
            var armActuator = GetBodyPart(mech, location)
                .TryGetSubEntities(SubEntitiesSelector.MOUNTS)
                .Where(e => e.Label == ArmActuatorLabel)
                .First();
            armActuator.HandleEvent(new GameEvent_Slot(mech, armActuator, mountable));
            return mech;
        }

        #endregion

        #region Bare Weapons

        public static Entity BuildMissile()
        {
            return BuildWeapon("Mssl.Rck.", MountSize.MEDIUM, 1, 15, 4, 100);
        }

        public static Entity BuildMiniMissile()
        {
            return BuildWeapon("Mn.Mssl.", MountSize.SMALL, 1, 10, 2, 75);
        }

        public static Entity BuildSniperRifle()
        {
            return BuildWeapon("Snpr.Rfl.", MountSize.LARGE, 6, 20, 8, 250);
        }

        public static Entity BuildRifle()
        {
            return BuildWeapon("Rfl.", MountSize.MEDIUM, 1, 12, 3, 80);
        }

        public static Entity BuildMachinegun()
        {
            return BuildWeapon("Mchngn.", MountSize.LARGE, -2, 14, 1, 20);
        }

        public static Entity BuildShotgun()
        {
            return BuildWeapon("Shtgn.", MountSize.MEDIUM, 3, 8, 6, 100);
        }

        public static Entity BuildPistol()
        {
            return BuildWeapon("Pstl.", MountSize.SMALL, 2, 8, 2, 65);
        }

        public static Entity BuildRockets()
        {
            return BuildWeapon("Rckt.Pd.", MountSize.SMALL, -3, 6, 9, 170);
        }

        public static Entity BuildDagger()
        {
            return BuildWeapon("Dggr.", MountSize.SMALL, 2, 1, 2, 15);
        }

        public static Entity BuildSword()
        {
            return BuildWeapon("Swrd.", MountSize.MEDIUM, 0, 1, 6, 30);
        }

        public static Entity BuildHammer()
        {
            return BuildWeapon("Hmmr.", MountSize.MEDIUM, -1, 1, 9, 60);
        }

        #endregion

        #region Slottable Parts

        public static Entity BuildMount(MountSize size)
        {
            var mount = new Entity(label: "Mount", typeLabel: EntityBuilder.SlottablePartTypeLabel)
                .AddComponent(new Component_Mount(size));
            switch (size)
            {
                case MountSize.SMALL:
                    mount.AddComponent(new Component_Slottable(2))
                        .AddComponent(new Component_InternalStructure(2));
                    break;
                case MountSize.MEDIUM:
                    mount.AddComponent(new Component_Slottable(4))
                        .AddComponent(new Component_InternalStructure(4));
                    break;
                case MountSize.LARGE:
                    mount.AddComponent(new Component_Slottable(8))
                        .AddComponent(new Component_InternalStructure(8));
                    break;
                default:
                    throw new ArgumentException("I have no idea how you passed " + size + " in.");
            }
            return mount;
        }

        public static Entity BuildHolster(MountSize size)
        {
            var mount = new Entity(label: "Mount", typeLabel: EntityBuilder.SlottablePartTypeLabel)
                .AddComponent(new Component_Holster(size));
            switch (size)
            {
                case MountSize.SMALL:
                    mount.AddComponent(new Component_Slottable(1))
                        .AddComponent(new Component_InternalStructure(2));
                    break;
                case MountSize.MEDIUM:
                    mount.AddComponent(new Component_Slottable(2))
                        .AddComponent(new Component_InternalStructure(4));
                    break;
                case MountSize.LARGE:
                    mount.AddComponent(new Component_Slottable(4))
                        .AddComponent(new Component_InternalStructure(8));
                    break;
                default:
                    throw new ArgumentException("I have no idea how you passed " + size + " in.");
            }
            return mount;
        }

        # region Naked Only

        public static Entity BuildPowerPlant()
        {
            return new Entity(label: "Pwr.Plnt.", typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_Slottable(5))
                .AddComponent(new Component_InternalStructure(5));
        }

        public static Entity BuildArmActuator()
        {
            return new Entity(label: ArmActuatorLabel, typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_Mount(MountSize.LARGE))
                .AddComponent(new Component_Slottable(2))
                .AddComponent(new Component_InternalStructure(2));
        }

        public static Entity BuildLegActuator()
        {
            return new Entity(label: "Leg.Actr.", typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_Slottable(3))
                .AddComponent(new Component_InternalStructure(3));
        }

        #endregion

        #region Weapons

        public static Entity BuildMountedMissile()
        {
            return BuildMountForWeapon(BuildMissile());
        }

        public static Entity BuildMountedMiniMissile()
        {
            return BuildMountForWeapon(BuildMiniMissile());
        }

        public static Entity BuildMountedSniperRifle()
        {
            return BuildMountForWeapon(BuildSniperRifle());
        }

        public static Entity BuildMountedRifle()
        {
            return BuildMountForWeapon(BuildRifle());
        }

        public static Entity BuildMountedMachinegun()
        {
            return BuildMountForWeapon(BuildMachinegun());
        }

        public static Entity BuildMountedShotgun()
        {
            return BuildMountForWeapon(BuildShotgun());
        }

        public static Entity BuildMountedPistol()
        {
            return BuildMountForWeapon(BuildPistol());
        }

        public static Entity BuildMountedRockets()
        {
            return BuildMountForWeapon(BuildRockets());
        }

        public static Entity BuildMountedDagger()
        {
            return BuildMountForWeapon(BuildDagger());
        }

        public static Entity BuildMountedSword()
        {
            return BuildMountForWeapon(BuildSword());
        }

        public static Entity BuildMountedHammer()
        {
            return BuildMountForWeapon(BuildHammer());
        }

        #endregion

        #region Other Parts

        public static Entity BuildAccelerator()
        {
            return new Entity(label: "Accel.", typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_AttributeModifierMult(EntityAttributeType.SPEED, .9))
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(1));
        }

        public static Entity BuildSensorPackage()
        {
            return new Entity(label: "Snsr.Pckg.", typeLabel: SlottablePartTypeLabel)
                .AddComponent(new Component_Slottable(2))
                .AddComponent(new Component_InternalStructure(2));
        }

        public static Entity BuildArmorPart()
        {
            return new Entity(label: "Armor", typeLabel: BodyPartTypeLabel)
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(4));
        }

        #endregion

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
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildLegActuator());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildLegActuator());

            // Slot in all the required components
            return mech;
        }

        #endregion

        public static Entity BuildArmoredMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BuildRifle());
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BuildRifle());

            SlotAt(mech, BodyPartLocation.HEAD, BuildMountedPistol());
            SlotAt(mech, BodyPartLocation.LEFT_ARM, BuildMountedPistol());
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, BuildMountedPistol());

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildKnifeMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BuildDagger());
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BuildDagger());

            SlotAt(mech, BodyPartLocation.LEFT_ARM, BuildMountedDagger());
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, BuildMountedDagger());
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildMountedDagger());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildMountedDagger());

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

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BuildSword());
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BuildSword());

            SlotAt(mech, BodyPartLocation.HEAD, BuildMountedPistol());

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildSniperMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BuildSniperRifle());
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BuildSniperRifle());

            SlotAt(mech, BodyPartLocation.LEFT_ARM, BuildHolsterForWeapon(BuildHammer()));
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, BuildHolsterForWeapon(BuildHammer()));

            FillLocationWith(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator);
            FillLocationWith(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator);

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildAlphaStrikerMech(string label, bool player)
        {
            var mech = BuildNakedMech(label, player);

            Func<Entity> buildRocketPod = () => BuildMountForWeapon(BuildRockets());

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BuildRockets());
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BuildRockets());

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
            var choice = rand.Next(4);
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
                case 4:
                    return BuildSniperMech(label, player);
                default:
                    return BuildNakedMech(label, player);
            }
        }

        #endregion
    }
}
