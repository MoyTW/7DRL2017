using Executor.AI.Combat;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Executor
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

        private static Entity BuildMountForWeapon(Entity weapon)
        {
            Entity mount;
            switch (weapon.GetComponentOfType<Component_Attachable>().SizeRequired)
            {
                case AttachmentSize.SMALL:
                    mount = BlueprintListing.BuildForLabel(Blueprints.SMALL_MOUNT);
                    break;
                case AttachmentSize.MEDIUM:
                    mount = BlueprintListing.BuildForLabel(Blueprints.MEDIUM_MOUNT);
                    break;
                case AttachmentSize.LARGE:
                    mount = BlueprintListing.BuildForLabel(Blueprints.LARGE_MOUNT);
                    break;
                default:
                    throw new ArgumentException("BuildMountForWeapon can't handle size: not SMALL, MEDIUM, LARGE");
            }

            mount.HandleEvent(new GameEvent_Slot(null, mount, weapon));
            return mount;
        }

        private static Entity BuildHolsterForWeapon(Entity weapon)
        {
            Entity holster;
            switch (weapon.GetComponentOfType<Component_Attachable>().SizeRequired)
            {
                case AttachmentSize.SMALL:
                    holster = BlueprintListing.BuildForLabel(Blueprints.SMALL_HOLSTER);
                    break;
                case AttachmentSize.MEDIUM:
                    holster = BlueprintListing.BuildForLabel(Blueprints.MEDIUM_HOLSTER);
                    break;
                case AttachmentSize.LARGE:
                    holster = BlueprintListing.BuildForLabel(Blueprints.LARGE_HOLSTER);
                    break;
                default:
                    throw new ArgumentException("BuildHolsterForWeapon can't handle size: not SMALL, MEDIUM, LARGE");
            }
            holster.HandleEvent(new GameEvent_Slot(null, holster, weapon));
            return holster;
        }

        private static Entity MountOntoArm(Entity mech, BodyPartLocation location, Entity mountable)
        {
            var armActuator = GetBodyPart(mech, location)
                .TryGetSubEntities(SubEntitiesSelector.SWAPPABLE_ATTACH_POINTS)
                .Where(e => e.Label == Blueprints.ARM_ACTUATOR)
                .First();
            armActuator.HandleEvent(new GameEvent_Slot(mech, armActuator, mountable));
            return mech;
        }

        #endregion

        #region Slottable Parts

        #region Weapons

        public static Entity BuildMountedMissile()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.MISSILE_RACK));
        }

        public static Entity BuildMountedMiniMissile()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.MINI_MISSILE));
        }

        public static Entity BuildMountedSniperRifle()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.SNIPER_RILFE));
        }

        public static Entity BuildMountedRifle()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.RIFLE));
        }

        public static Entity BuildMountedMachinegun()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.MACHINEGUN));
        }

        public static Entity BuildMountedShotgun()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.SHOTGUN));
        }

        public static Entity BuildMountedPistol()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.PISTOL));
        }

        public static Entity BuildMountedRockets()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.ROCKET_POD));
        }

        public static Entity BuildMountedDagger()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.DAGGER));
        }

        public static Entity BuildMountedSword()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.SWORD));
        }

        public static Entity BuildMountedHammer()
        {
            return BuildMountForWeapon(BlueprintListing.BuildForLabel(Blueprints.HAMMER));
        }

        #endregion

        #region Other Parts

        public static Entity BuildAccelerator()
        {
            return BlueprintListing.BuildForLabel(Blueprints.ACCELERATOR);
        }

        public static Entity BuildSensorPackage()
        {
            return BlueprintListing.BuildForLabel(Blueprints.SENSOR_PACKAGE);
        }

        public static Entity BuildArmorPart()
        {
            return BlueprintListing.BuildForLabel(Blueprints.ARMOR);
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

        public static Entity BuildNakedMech(string label, bool player, Entity pilot, Guidebook book)
        {
            var mech = new Entity(label: label, typeLabel: MechTypeLabel)
                .AddComponent(new Component_Piloted(pilot))
                .AddComponent(new Component_MechSkeleton())
                .AddComponent(new Component_Attacker());

            if (player)
                mech.AddComponent(new Component_Player());
            else if (book != null)
                mech.AddComponent(new Component_AI(book));
            else
                mech.AddComponent(new Component_AI());

            SlotAt(mech, BodyPartLocation.HEAD, BuildSensorPackage());
            SlotAt(mech, BodyPartLocation.TORSO, BlueprintListing.BuildForLabel(Blueprints.POWER_PLANT));
            SlotAt(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.ARM_ACTUATOR));
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.ARM_ACTUATOR));
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BlueprintListing.BuildForLabel(Blueprints.LEG_ACTUATOR));
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BlueprintListing.BuildForLabel(Blueprints.LEG_ACTUATOR));

            // Slot in all the required components
            return mech;
        }

        #endregion

        public static Entity BuildArmoredMech(string label, bool player, Guidebook book=null)
        {
            var mech = BuildNakedMech(label, player, new Entity(), book);

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.RIFLE));
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.RIFLE));

            SlotAt(mech, BodyPartLocation.HEAD, BuildMountedPistol());
            SlotAt(mech, BodyPartLocation.LEFT_ARM, BuildMountedPistol());
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, BuildMountedPistol());

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildKnifeMech(string label, bool player, Guidebook book=null)
        {
            var mech = BuildNakedMech(label, player, new Entity(), book);

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.DAGGER));
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.DAGGER));

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

        public static Entity BuildPaladinMech(string label, bool player, Guidebook book=null)
        {
            var mech = BuildNakedMech(label, player, new Entity(), book);

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.SWORD));
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.SWORD));

            SlotAt(mech, BodyPartLocation.HEAD, BuildMountedPistol());

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildSniperMech(string label, bool player, Guidebook book=null)
        {
            var mech = BuildNakedMech(label, player, new Entity(), book);

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.SNIPER_RILFE));
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.SNIPER_RILFE));

            SlotAt(mech, BodyPartLocation.LEFT_ARM,
                BuildHolsterForWeapon(BlueprintListing.BuildForLabel(Blueprints.HAMMER)));
            SlotAt(mech, BodyPartLocation.RIGHT_ARM,
                BuildHolsterForWeapon(BlueprintListing.BuildForLabel(Blueprints.HAMMER)));

            FillLocationWith(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator);
            FillLocationWith(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator);

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildAlphaStrikerMech(string label, bool player, Guidebook book=null)
        {
            var mech = BuildNakedMech(label, player, new Entity(), book);

            Func<Entity> buildRocketPod = () => BuildMountForWeapon(
                BlueprintListing.BuildForLabel(Blueprints.ROCKET_POD));

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.ROCKET_POD));
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.ROCKET_POD));

            FillLocationWith(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator);
            FillLocationWith(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator);

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, buildRocketPod);
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildSlowKnifeMech(string label, bool player, Guidebook book=null)
        {
            var mech = BuildNakedMech(label, player, new Entity(), book);

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.DAGGER));
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.DAGGER));

            SlotAt(mech, BodyPartLocation.LEFT_ARM, BuildMountedDagger());
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, BuildMountedDagger());
            SlotAt(mech, BodyPartLocation.LEFT_LEG, BuildMountedDagger());
            SlotAt(mech, BodyPartLocation.RIGHT_LEG, BuildMountedDagger());

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildFastPistolMech(string label, bool player, Guidebook book=null)
        {
            var mech = BuildNakedMech(label, player, new Entity(), book);

            MountOntoArm(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.PISTOL));
            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.PISTOL));


            FillLocationWith(mech, BodyPartLocation.LEFT_LEG, BuildAccelerator);
            FillLocationWith(mech, BodyPartLocation.RIGHT_LEG, BuildAccelerator);

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        public static Entity BuildRandomMech(string label, bool player, IRandom rand, Guidebook book=null)
        {
            var choice = rand.Next(4);
            switch (choice)
            {
                case 0:
                    return BuildArmoredMech(label, player, book);
                case 1:
                    return BuildKnifeMech(label, player, book);
                case 2:
                    return BuildPaladinMech(label, player, book);
                case 3:
                    return BuildAlphaStrikerMech(label, player, book);
                case 4:
                    return BuildSniperMech(label, player, book);
                default:
                    return BuildNakedMech(label, player, new Entity(), book);
            }
        }

        #endregion
    }
}
