using Executor.AI;
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

        private static Entity BuildMountForWeapon(Entity mech, BodyPartLocation location, Entity weapon)
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

            SlotAt(mech, location, mount);
            mount.HandleEvent(new GameEvent_Slot(mech, mount, weapon));
            return mount;
        }

        private static Entity SlotHolsterForWeapon(Entity mech, BodyPartLocation location, Entity weapon)
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
            SlotAt(mech, location, holster);
            holster.HandleEvent(new GameEvent_Slot(mech, holster, weapon));
            return holster;
        }

        public static Entity MountOntoArm(Entity mech, BodyPartLocation location, Entity mountable)
        {
            var armActuator = GetBodyPart(mech, location)
                .TryGetSubEntities(SubEntitiesSelector.SWAPPABLE_ATTACH_POINTS)
                .Where(e => e.Label == Blueprints.HAND)
                .First();
            armActuator.HandleEvent(new GameEvent_Slot(mech, armActuator, mountable));
            return mech;
        }

        #endregion

        #region Slottable Parts

        public static Entity BuildHelmet()
        {
            return new Entity(label: "Helmet", typeLabel: "Armor")
                .AddComponent(new Component_Slottable(2))
                .AddComponent(new Component_InternalStructure(10));
        }

        public static Entity BuildArmorPart()
        {
            return BlueprintListing.BuildForLabel(Blueprints.ARMOR);
        }

        public static Entity BuildPhoneScanner()
        {
            return new Entity(label: "Phone Scanner", typeLabel: "Scanner")
                .AddComponent(new Component_Attachable(AttachmentSize.SMALL))
                .AddComponent(new Component_AttributeModifier(EntityAttributeType.DETECTION_RADIUS, ModifierType.FLAT, 2));
        }

        public static Entity BuildHandheldScanner()
        {
            return new Entity(label: "Hand Scanner", typeLabel: "Scanner")
                .AddComponent(new Component_Attachable(AttachmentSize.SMALL))
                .AddComponent(new Component_AttributeModifier(EntityAttributeType.DETECTION_RADIUS, ModifierType.FLAT, 3));
        }

        public static Entity BuildDutyScanner()
        {
            return new Entity(label: "Duty Scanner", typeLabel: "Scanner")
                .AddComponent(new Component_Attachable(AttachmentSize.SMALL))
                .AddComponent(new Component_AttributeModifier(EntityAttributeType.DETECTION_RADIUS, ModifierType.FLAT, 4));
        }

        #endregion

        #region Base

        public static Entity BuildBodyPart(BodyPartLocation location, int slotSpace, int internalStructure)
        {
            return new Entity(label: location.ToString(), typeLabel: "BodyPart")
                .AddComponent(new Component_BodyPartLocation(location))
                .AddComponent(new Component_SlottedContainer(slotSpace))
                .AddComponent(new Component_SlottedStructure())
                .AddComponent(new Component_InternalStructure(internalStructure));
        }

        public static Entity BuildNakedMech(string label, bool player, Guidebook book)
        {
            var mech = new Entity(label: label, typeLabel: MechTypeLabel)
                .AddComponent(new Component_Buffable())
                .AddComponent(new Component_ActionExecutor(Config.DefaultEntityAP))
                .AddComponent(new Component_Skeleton());

            if (player)
            {
                mech.AddComponent(new Component_Player());
                mech.AddComponent(new Component_FocusUser());
            }
            else if (book != null)
                mech.AddComponent(new Component_AI(book));
            else
                throw new InvalidOperationException("book can't be null!");
            
            SlotAt(mech, BodyPartLocation.LEFT_ARM, BlueprintListing.BuildForLabel(Blueprints.HAND));
            SlotAt(mech, BodyPartLocation.RIGHT_ARM, BlueprintListing.BuildForLabel(Blueprints.HAND));

            // Slot in all the required components
            return mech;
        }

        public static Entity BuildPlayerEntity()
        {
            var mech = BuildNakedMech("You", true, null);

            MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildHFBlade());

            foreach (var location in EntityBuilder.MechLocations)
            {
                FillLocationWith(mech, location, BuildArmorPart);
            }

            return mech;
        }

        #endregion
        
    }
}
